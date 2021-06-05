using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Messenger.Web.Models
{
    public class Message
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime TimeStamp { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
    }
}
