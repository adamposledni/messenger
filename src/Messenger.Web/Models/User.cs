using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Models
{
    public class UserCreateReq
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserRes
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserNameRes
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserAuthReq
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserAuthRes
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public UserAuthRes(string email, string token)
        {
            Email = email;
            Token = token;
        }

    }
}
