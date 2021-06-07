using Messenger.Web.Models;
using Messenger.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNameRes>>> ListOthers()
        {
            string userId = HttpContext.User.Identity.Name;
            var response = await _userService.ListOthersAsync(userId);
            return StatusCode(200, response);
        }

        // POST: api/users
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserRes>> Create([FromBody] UserCreateReq body)
        {
            var response = await _userService.CreateAsync(body);
            return StatusCode(201, response);
        }

        // POST: api/users/auth
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<ActionResult<UserAuthRes>> Authenticate([FromBody] UserAuthReq body)
        {
            var response = await _userService.AuthenticateAsync(body);
            return StatusCode(200, response);
        }


    }
}
