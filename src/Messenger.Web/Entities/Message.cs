using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Messenger.Web.Models
{
    /// <summary>
    /// Message model
    /// </summary>
    public class Message
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime TimeStamp { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
    }
}
