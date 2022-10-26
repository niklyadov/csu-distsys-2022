using Microsoft.EntityFrameworkCore;
using TestApp.DAL;
using TestApp.DAL.QueryBuilders;
using TestApp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine(builder.Configuration.GetConnectionString("Default"));

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