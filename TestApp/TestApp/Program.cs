using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client.Core.DependencyInjection;
using TestApp;
using TestApp.DAL;
using TestApp.DAL.QueryBuilders;
using TestApp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppConfiguration"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["AppConfiguration:AppName"],
        Version = "v1"
    });
});

var rabbitMqSection = builder.Configuration.GetSection("RabbitMq");
var exchangeSection = builder.Configuration.GetSection("RabbitMqExchange");

builder.Services.AddRabbitMqServices(rabbitMqSection)
    .AddProductionExchange("test-app", exchangeSection);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<LinksQueryBuilder>();
builder.Services.AddScoped<LinkService>();
builder.Services.AddTransient<MarkAppNameMiddleware>();

var app = builder.Build();

app.UseMiddleware<MarkAppNameMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json",builder.Configuration["AppConfiguration:AppName"]);
});

app.UseAuthorization();

app.MapControllers();

app.Run();