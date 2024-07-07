using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using TestMongoDB.Abstractions;
using TestMongoDB.Models;

namespace TestMongoDB;

public static class SeedData
{
    public static List<User> Users => new()
    {
        new User
        {
            Id = ObjectId.GenerateNewId(),
            UserName = "john_doe",
            PasswordHash = "hashed_password",
            Email = "john.doe@example.com"
        },
        new User
        {
            Id = ObjectId.GenerateNewId(),
            UserName = "jane_smith",
            PasswordHash = "hashed_password",
            Email = "jane.smith@example.com"
        }
    };

    public static List<Product> Products => new()
    {
        new Product
        {
            Id = ObjectId.GenerateNewId(),
            Name = "Sample Product 1",
            Description = "This is a sample product description.",
            Price = 19.99,
            Stock = 50,
            CreatedAt = DateTime.UtcNow,
            Reviews = new List<Review>
            {
                new Review
                {
                    Comment = "Great product!",
                    Rating = 5,
                    CreatedAt = DateTime.UtcNow,
                    UserId = Users.First().Id,
                    ExtraElements = new BsonDocument
                    {
                        { "username", "john_doe" }
                    }
                },
                new Review
                {
                    Comment = "Not bad, could be better.",
                    Rating = 3,
                    CreatedAt = DateTime.UtcNow,
                    UserId = Users[1].Id,
                    ExtraElements = new BsonDocument
                    {
                        { "username", "jane_smith" }
                    }
                }
            }
        },
        new Product
        {
            Id = ObjectId.GenerateNewId(),
            Name = "Sample Product 2",
            Description = "Another sample product.",
            Price = 29.99,
            Stock = 30,
            CreatedAt = DateTime.UtcNow,
            Reviews = new List<Review>
            {
                new Review
                {
                    Comment = "Decent product.",
                    Rating = 4,
                    CreatedAt = DateTime.UtcNow,
                    UserId = ObjectId.GenerateNewId(),
                    ExtraElements = new BsonDocument
                    {
                        { "username", "john_doe" }
                    }
                }
            }
        }
    };

    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
        var isAborted = false;

        using var session = mongoClient.StartSession();
        session.StartTransaction();

        var userResult = await userRepository.InsertManyAsync(Users, session);
        var productResult = await productRepository.InsertManyAsync(Products, session);

        if (userResult.IsFailure || productResult.IsFailure)
        {
            isAborted = true;
            session.AbortTransaction();
        }
        else
        {
            await session.CommitTransactionAsync();
        }

        if (!isAborted)
        {
            Console.WriteLine("Successfully committed transaction!");
        }
        else
        {
            Console.WriteLine("Transaction aborted!");
        }
    }
}
