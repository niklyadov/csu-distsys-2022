using RabbitMQ.Client.Core.DependencyInjection;
using StackExchange.Redis;
using TestApp.Worker;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var rabbitMqSection = hostContext.Configuration.GetSection("RabbitMq");
        var exchangeSection = hostContext.Configuration.GetSection("RabbitMqExchange");
        
        services.AddSingleton<IDatabase>(cfg 
            => ConnectionMultiplexer
                .Connect(hostContext.Configuration.GetConnectionString("redis"))
                .GetDatabase());
        
        services.AddRabbitMqServices(rabbitMqSection)
            .AddConsumptionExchange("test-app", exchangeSection)
            .AddMessageHandlerTransient<LinksPrepareMessageHandler>("links-prepare");

        services.Configure<AppConfiguration>(hostContext.Configuration.GetSection(nameof(AppConfiguration)));

    })
    .Build();

await host.RunAsync();