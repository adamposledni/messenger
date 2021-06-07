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
        /// <summary>
        /// Method name to receive message
        /// </summary>
        Task ReceiveMessage(MessageRes message);
    }

    /// <summary>
    /// Hub for chatting
    /// </summary>
    public class ChatHub : Hub<IChatHub>
    {
        private IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Sending message to corresponding users
        /// </summary>
        [Authorize]
        public async Task SendMessage(MessageReq incoming)
        {
            // caller user ID
            string userId = Context.User.Identity.Name;
            // insert message into DB and get DTO
            var outcomming = await _chatService.CreateMessagesAsync(incoming, userId);
            // get corresponsing chat members IDs
            var membersIds = await _chatService.GetOtherMembersIdsAsync(incoming.ChatId, userId);

            // send message to chat members
            await Clients.Users(membersIds).ReceiveMessage(outcomming);

            // send message to the caller
            outcomming.IsOwn = true;
            await Clients.Caller.ReceiveMessage(outcomming);
        }
    }
}
