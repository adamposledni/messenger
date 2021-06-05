using AutoMapper;
using Messenger.Web.Entities;
using Messenger.Web.Helpers;
using Messenger.Web.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Services
{
    public interface IChatService
    {
        Task<IEnumerable<ChatSimpleRes>> GetAllAsync(string userId);
        Task<bool> CreateAsync(ChatReq model, string userId);
        Task<IEnumerable<MessageRes>> GetMessagesAsync(string chatId, string userId);
        Task<MessageRes> CreateMessagesAsync(MessageReq model, string userId);
        Task<IEnumerable<string>> GetOtherMembersIdsAsync(string chatId, string userId);
    }

    public class ChatService : IChatService
    {
        private IMongoCollection<Chat> _chatCollection;
        private IMapper _mapper;
        private IUserService _userService;
        public ChatService(IMongoCollection<Chat> chatCollection, IMapper mapper, IUserService userService)
        {
            _chatCollection = chatCollection;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IEnumerable<ChatSimpleRes>> GetAllAsync(string userId)
        {
            // all users chats
            var chats = await _chatCollection.Find(ch => ch.MemberIds.Any(mid => mid == userId)).ToListAsync();
            List<ChatSimpleRes> formattedChats = new();
            foreach (var chat in chats)
            {
                // chat without name
                if (chat.Name == null)
                {
                    chat.Name = await GetChatNameAsync(chat, userId);
                }
                ChatSimpleRes formattedChat = _mapper.Map<ChatSimpleRes>(chat);
                formattedChat.IsGroup = chat.MemberIds.Count() > 2;
                formattedChats.Add(formattedChat);
            }

            return formattedChats;
        }

        public async Task<bool> CreateAsync(ChatReq model, string userId)
        {
            List<string> modelMemberIds = model.MemberIds.ToList();
            modelMemberIds.Add(userId);

            // remove duplicates
            modelMemberIds = modelMemberIds.Distinct().ToList();

            // check if enought members
            if (modelMemberIds.Count() < 2)
            {
                throw new AppLogicException("Chat has less then 2 members");
            }

            // check if already exists
            bool alreadyExists = false;
            var chats = await _chatCollection.Find(ch => true).ToListAsync();
            foreach (var chat in chats)
            {
                var set = new HashSet<string>(chat.MemberIds);
                if (set.SetEquals(modelMemberIds))
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                throw new AppLogicException("Chat with these members already exists");
            }

            Chat newChat = new()
            {
                MemberIds = modelMemberIds,
                Name = model.Name,
                Messages = new List<Message>()
            };
            await _chatCollection.InsertOneAsync(newChat);
            return true;
        }

        public async Task<IEnumerable<MessageRes>> GetMessagesAsync(string chatId, string userId)
        {
            var chat = await _chatCollection.Find(ch => ch.Id == chatId).FirstOrDefaultAsync();

            // check if the chat exists
            if (chat == null)
            {
                return null;
            }

            // check if it belongs to the user
            if (!chat.MemberIds.Contains(userId))
            {
                throw new AppLogicException("User is not member of this chat");
            }

            // format response
            List<MessageRes> messages = new();
            Dictionary<string, UserNameRes> authors = new();
            foreach (var message in chat.Messages)
            {
                UserNameRes author;
                bool knownAuthor = authors.TryGetValue(message.AuthorId, out author);
                if (!knownAuthor)
                {
                    var user = _mapper.Map<UserNameRes>(
                        await _userService.GetUserByIdAsync(message.AuthorId));
                    authors.Add(message.AuthorId, user);
                    author = user;
                }

                MessageRes formattedMessage = new()
                {
                    Content = message.Content,
                    TimeStamp = message.TimeStamp,
                    Author = author,
                    IsOwn = author.Id == userId
                };

                messages.Add(formattedMessage);
            }

            return messages;
        }

        public async Task<MessageRes> CreateMessagesAsync(MessageReq model, string authorId)
        {
            var chat = await _chatCollection.Find(ch => ch.Id == model.ChatId).FirstOrDefaultAsync();

            // check if the chat exists
            if (chat == null)
            {
                throw new AppLogicException("Chat does not exist");
            }

            // check if it belongs to the user
            if (!chat.MemberIds.Contains(authorId))
            {
                throw new AppLogicException("User is not member of this chat");
            }

            // add message to DB
            Message message = new()
            {
                Content = model.Content,
                AuthorId = authorId,
                TimeStamp = DateTime.UtcNow
            };
            var updateDefinition = Builders<Chat>.Update.Push(ch => ch.Messages, message);
            await _chatCollection.UpdateOneAsync(ch => ch.Id == model.ChatId, updateDefinition);

            // format response
            var author = _mapper.Map<UserNameRes>(
                await _userService.GetUserByIdAsync(authorId));

            MessageRes response = new()
            {
                Content = model.Content,
                Author = author,
                TimeStamp = DateTime.UtcNow,
                ChatId = model.ChatId,
                IsOwn = false
            };

            return response;
        }

        public async Task<IEnumerable<string>> GetOtherMembersIdsAsync(string chatId, string userId)
        {
            var chat = await _chatCollection.Find(ch => ch.Id == chatId).FirstOrDefaultAsync();

            // check if the chat exists
            if (chat == null)
            {
                throw new AppLogicException("Chat does not exist");
            }

            var otherMembersIds = chat.MemberIds.ToList();
            otherMembersIds.Remove(userId);

            return otherMembersIds;
        }

        private async Task<string> GetChatNameAsync(Chat chat, string userId)
        {
            List<string> memberIds = chat.MemberIds.ToList();
            memberIds.Remove(userId);

            int memberIdsCount = memberIds.Count();

            // only one member
            if (memberIdsCount == 1)
            {
                string id = memberIds[0];
                var user = await _userService.GetUserByIdAsync(id);
                return $"{user.FirstName} {user.LastName}";
            }

            // more members
            string name = "";
            for (int i = 0; i < memberIdsCount; i++)
            {
                var id = memberIds[i];
                var user = await _userService.GetUserByIdAsync(id);
                name += user.FirstName;
                if (i < memberIdsCount - 1)
                {
                    name += ", ";
                }
            }
            return name;
        }


    }
}
