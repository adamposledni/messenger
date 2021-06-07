using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Models
{
    /// <summary>
    /// User request for creation
    /// </summary>
    public class UserCreateReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /// <summary>
    /// User response
    /// </summary>
    public class UserRes
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /// <summary>
    /// User response (only name)
    /// </summary>
    public class UserNameRes
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /// <summary>
    /// User request for authentication
    /// </summary>
    public class UserAuthReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// User response for authentication
    /// </summary>
    public class UserAuthRes
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public UserAuthRes(string username, string token)
        {
            Username = username;
            Token = token;
        }
    }
}
