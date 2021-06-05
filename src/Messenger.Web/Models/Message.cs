using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Models
{
    public class MessageRes
    {
        public string ChatId { get; set; }
        public UserNameRes Author { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsOwn { get; set; }
    }

    public class MessageReq
    {
        public string ChatId { get; set; }
        public string Content { get; set; }
    }
}
