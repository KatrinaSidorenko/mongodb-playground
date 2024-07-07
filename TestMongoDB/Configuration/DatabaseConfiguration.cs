using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TestMongoDB.Models.Settings;

namespace TestMongoDB.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        ClassMapRegistartion.Register();
        services.Configure<MongoDBSettings>(configuration.GetSection(nameof(MongoDBSettings)));
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
            return new MongoClient(settings.DefaultConnection);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
            return client.GetDatabase(settings.DatabaseName);
        });

        return services;
    }
}
