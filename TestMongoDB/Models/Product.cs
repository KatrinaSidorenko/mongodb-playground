using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestMongoDB.Models;

public class Product : IEntityWithId
{
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    [BsonIgnoreIfNull] // This field will be ignored during serialization if it is null
    public string Description { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("reviews")]
    [BsonIgnoreIfNull]
    public List<Review> Reviews { get; set; }
}
