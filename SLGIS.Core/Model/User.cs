using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SLGIS.Core
{
    [BsonIgnoreExtraElements]
    public class User : MongoUser
    {
        public string Name { get; set; }

        public DateTime Updated { get; set; }
        public bool IsLocked { get; set; }
    }
}
