using Messenger.Web.Entities;
using Messenger.Web.Hubs;
using Messenger.Web.Models;
using Messenger.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Controllers
{

    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ChatRes>>> GetAll()
        {
            string userId = HttpContext.User.Identity.Name;
            var response = await _chatService.GetAllAsync(userId);
            return StatusCode(200, response);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create([FromBody] ChatReq body)
        {
            string userId = HttpContext.User.Identity.Name;
            await _chatService.CreateAsync(body, userId);
            return StatusCode(204);
        }

        [HttpGet("{chatId}/messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageRes>>> GetMessages([FromRoute] string chatId)
        {
            string userId = HttpContext.User.Identity.Name;
            var response = await _chatService.GetMessagesAsync(chatId, userId);
            // chat does not exist
            if (response == null)
            {
                return StatusCode(404);
            }
            return StatusCode(200, response);
        }
    }
}
