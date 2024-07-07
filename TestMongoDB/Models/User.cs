using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestMongoDB.Models;

public class User : IEntityWithId
{
    public ObjectId Id { get; set; }

    [BsonElement("username")] // This is the name of the field in the MongoDB collection during serialization
    public string UserName { get; set; }

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }
}
