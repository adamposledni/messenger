using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Models
{
    /// <summary>
    /// Message response
    /// </summary>
    public class MessageRes
    {
        public string ChatId { get; set; }
        public UserNameRes Author { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsOwn { get; set; }
    }

    /// <summary>
    /// Message request
    /// </summary>
    public class MessageReq
    {
        public string ChatId { get; set; }
        public string Content { get; set; }
    }
}
