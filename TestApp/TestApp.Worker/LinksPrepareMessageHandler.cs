using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace TestApp.Worker;

public class Worker : IMessageHandler
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient = new ();

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public void Handle(MessageHandlingContext context, string matchingRoute)
    {
        var message = context.Message.GetMessage(); 
        
        Task.Run(() => Task.FromResult(WorkWithRequestTask(JsonSerializer.Deserialize<Link>(message))));
    }

    private async Task WorkWithRequestTask(Link? link)
    {
        try
        {
            if (link == null) throw new ArgumentNullException(nameof(link));
        
            var response = await _httpClient.GetAsync(link.Url);

            link.HttpStatusCode = (int)response.StatusCode;
            link.ExecutedAt = DateTime.UtcNow;

            _logger.LogInformation("{Link} response code {Code}", link, response.StatusCode);

            _logger.LogInformation("{Link}", link);
            await UpdateLinkRequestAsync(link);

        }
        catch (Exception exception)
        {
            _logger.LogError("{Exception}", exception);
        }
    }
    
    private async Task UpdateLinkRequestAsync(Link link, string url = "http://test-app:8000/link")
    {
        using HttpContent content = new StringContent(JsonSerializer.Serialize(link), Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await _httpClient.PutAsync(url, content).ConfigureAwait(false);

        await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}

public record Link
{   
    public long Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDateTime { get; set; }
    public string Url { get; set; }
    public int? HttpStatusCode { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
}