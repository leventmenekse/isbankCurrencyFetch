using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Models
{
    class CurrencyModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("RateSell")]
        public decimal RateSell { get; set; }

        [BsonElement("RateBuy")]
        public decimal RateBuy { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        public CurrencyModel()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
