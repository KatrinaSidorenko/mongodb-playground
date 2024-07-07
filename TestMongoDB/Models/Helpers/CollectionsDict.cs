namespace TestMongoDB.Models.Helpers;

public static class CollectionsDict
{
    public static Dictionary<Type, string> CollectionNames => new()
    {
        { typeof(User), "users" },
        { typeof(Product), "products" }
    };
}
