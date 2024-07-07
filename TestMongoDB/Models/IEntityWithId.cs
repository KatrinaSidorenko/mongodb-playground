using MongoDB.Bson;

namespace TestMongoDB.Models;

public interface IEntityWithId
{
    ObjectId Id { get; set; }
}
