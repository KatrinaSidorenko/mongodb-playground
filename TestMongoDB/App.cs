using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using TestMongoDB.Abstractions;
using TestMongoDB.Models;

namespace TestMongoDB;

public class App(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task RunAsync()
    {
        //Plyground code
        //using var scope = _serviceProvider.CreateScope();
        //var productRepository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

        //var product = await productRepository.GetByIdAsync(ObjectId.Parse("668a54c28b05c00d8ae158ea"));
        //product.Value.Reviews.ElementAt(0).ExtraElements.TryGetValue("username", out var username);
        //Console.WriteLine(username);

        //Transactions
        using var scope = _serviceProvider.CreateScope();
        var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
        var isAborted = false;

        using var session = mongoClient.StartSession();
        
        try
        {
            
            var user = new User
            {
                Id = ObjectId.GenerateNewId(),
                UserName = "kola_hola",
                PasswordHash = "hashed_password",
                Email = "kola_hola@example.com"
            };

            var product = new Product
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Sample Product 3",
                Description = "This is a sample product description.",
                Price = 19.99,
                Stock = 50,
                CreatedAt = DateTime.UtcNow,
                Reviews = new List<Review>
                {
                    new Review
                    {
                        Comment = "Great product!",
                        Rating = 4,
                        CreatedAt = DateTime.UtcNow,
                        UserId = user.Id,
                        ExtraElements = new BsonDocument
                        {
                            { "username", "kola_hola" }
                        }
                    }
                }
            };

            var newReview = new Review
            {
                Comment = "Awesome product!",
                Rating = 5,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                ExtraElements = new BsonDocument
                {
                    { "username", "kola_hola" }
                }
            };

            session.StartTransaction();

            Console.WriteLine(session.ServerSession.Id);
            var userResult = await userRepository.CreateAsync(user, session);
            Console.WriteLine(session.ServerSession.Id);

            var productResult = await productRepository.CreateAsync(product, session);
            Console.WriteLine(session.ServerSession.Id);


            var reviewResult = await productRepository.AddSubdocumentAsync(product.Id, p => p.Reviews, newReview, session);

            if (userResult.IsFailure || productResult.IsFailure || reviewResult.IsFailure)
            {
                await session.AbortTransactionAsync();
                isAborted = true;
            }
            else
            {
                await session.CommitTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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
