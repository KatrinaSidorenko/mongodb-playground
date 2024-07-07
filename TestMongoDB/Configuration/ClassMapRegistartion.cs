using MongoDB.Bson.Serialization;
using TestMongoDB.Models;

namespace TestMongoDB.Configuration;

public static class ClassMapRegistartion
{
    public static void Register()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
        });

        BsonClassMap.RegisterClassMap<Review>(cm =>
        {
            cm.AutoMap();
            cm.MapExtraElementsMember(c => c.ExtraElements);
        });

        BsonClassMap.RegisterClassMap<Product>(cm =>
        {
            cm.AutoMap();
        });
    }
}
