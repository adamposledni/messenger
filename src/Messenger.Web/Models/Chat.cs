using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Models
{
    public class ChatSimpleRes
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
    }

    public class ChatReq
    {
        public string Name { get; set; }
        public IEnumerable<string> MemberIds { get; set; }
    }
}
