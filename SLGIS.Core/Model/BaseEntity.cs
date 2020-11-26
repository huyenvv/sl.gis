using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Text.Json.Serialization;

namespace SLGIS.Core
{
    public class BaseEntity
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

        [JsonIgnore]
        public DateTime Created { get; protected set; } = DateTime.UtcNow.AddHours(7);

        public string CreatedBy { get; set; }
    }
}
