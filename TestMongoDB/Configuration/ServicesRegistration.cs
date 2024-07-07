using Microsoft.Extensions.DependencyInjection;
using TestMongoDB.Abstractions;
using TestMongoDB.Services;

namespace TestMongoDB.Configuration;

public static class ServicesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        return services;
    }
}
