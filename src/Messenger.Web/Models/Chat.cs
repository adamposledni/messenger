using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Models
{
    /// <summary>
    /// Chat response
    /// </summary>
    public class ChatRes
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
    }

    /// <summary>
    /// Chat request
    /// </summary>
    public class ChatReq
    {
        public string Name { get; set; }

        [JsonProperty("memberIds")]
        public IEnumerable<string> MemberIds { get; set; }
    }
}
