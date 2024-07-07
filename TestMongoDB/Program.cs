using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TestMongoDB.Configuration;
using TestMongoDB;

//Set up the depency injection in the Program.cs
var services = new ServiceCollection();

//Set up the reading of the config.json file
var configuration = new ConfigurationBuilder()
    .AddJsonFile("config.json", optional: true, reloadOnChange: true)
    .Build();

services.AddDatabase(configuration);
services.AddRepositories();
services.AddSingleton<App>();

var serviceProvider = services.BuildServiceProvider();
//serviceProvider.SeedAsync().GetAwaiter().GetResult(); <-- Seed data to existing database and collections

var app = serviceProvider.GetRequiredService<App>();
app.RunAsync().GetAwaiter().GetResult();



