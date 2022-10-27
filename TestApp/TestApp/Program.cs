using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Core.DependencyInjection;
using TestApp.DAL;
using TestApp.DAL.QueryBuilders;
using TestApp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitMqSection = builder.Configuration.GetSection("RabbitMq");
var exchangeSection = builder.Configuration.GetSection("RabbitMqExchange");

builder.Services.AddRabbitMqServices(rabbitMqSection)
    .AddProductionExchange("test-app", exchangeSection);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<LinksQueryBuilder>();
builder.Services.AddScoped<LinkService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => 
    c.SwaggerEndpoint("/swagger/v1/swagger.json","Test App")); 

app.UseAuthorization();

app.MapControllers();

app.Run();