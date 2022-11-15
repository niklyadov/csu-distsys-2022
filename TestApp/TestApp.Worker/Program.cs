using RabbitMQ.Client.Core.DependencyInjection;
using TestApp.Worker;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var rabbitMqSection = hostContext.Configuration.GetSection("RabbitMq");
        var exchangeSection = hostContext.Configuration.GetSection("RabbitMqExchange");

        services.AddRabbitMqServices(rabbitMqSection)
            .AddConsumptionExchange("test-app", exchangeSection)
            .AddMessageHandlerTransient<Worker>("links-prepare");

        services.Configure<ApiSettings>(hostContext.Configuration.GetSection(nameof(ApiSettings)));

    })
    .Build();

await host.RunAsync();