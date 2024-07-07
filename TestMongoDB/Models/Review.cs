using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestMongoDB.Models;

public class Review
{
    [BsonElement("comment")]
    [BsonIgnoreIfNull]
    public string Comment { get; set; }

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonExtraElements]
    public BsonDocument ExtraElements { get; set; }
}
