using Messenger.Web.Entities;
using Messenger.Web.Models;
using Messenger.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(MessageRes message);
    }

    public class ChatHub : Hub<IChatHub>
    {
        //private IMongoCollection<Chat> _chatCollection;
        private IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        [Authorize]
        public async Task SendMessage(MessageReq incoming)
        {
            string userId = Context.User.Identity.Name;
            var outcomming = await _chatService.CreateMessagesAsync(incoming, userId);
            var membersIds = await _chatService.GetOtherMembersIdsAsync(incoming.ChatId, userId);

            await Clients.Users(membersIds).ReceiveMessage(outcomming);

            outcomming.IsOwn = true;
            await Clients.Caller.ReceiveMessage(outcomming);
        }
    }
}
