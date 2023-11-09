using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CatShopApi.Models
{
    public class Cat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("name")]
        public  string name { get; set; } 

        public  decimal price { get; set; }

        public string breed { get; set; }

        public int age { get; set; }

        public int index { get; set; }
    }

    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Text { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CatId { get; set; } // Ссылка на кота
    }
}

public class CatShopDatabaseSettings
{
    public CatShopDatabaseSettings(string connectionString, string databaseName, string catCollectionName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
        CatCollectionName = catCollectionName;
    }

    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CatCollectionName { get; set; } = null!;
}


