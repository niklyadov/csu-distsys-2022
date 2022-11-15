using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace TestApp.Worker;

public class LinksPrepareMessageHandler : IMessageHandler
{
    private readonly ILogger<LinksPrepareMessageHandler> _logger;
    private readonly LinksPrepareService _linksPrepareService;
    private readonly AppConfiguration _appConfiguration;
    private readonly HttpClient _httpClient = new ();

    public LinksPrepareMessageHandler(ILogger<LinksPrepareMessageHandler> logger, IOptions<AppConfiguration> apiSettings, 
        LinksPrepareService linksPrepareService)
    {
        _logger = logger;
        _linksPrepareService = linksPrepareService;
        _appConfiguration = apiSettings.Value;
    }

    public void Handle(MessageHandlingContext context, string matchingRoute)
    {
        var message = context.Message.GetMessage(); 
        
        Task.Run(async () =>
        {
            var link = await _linksPrepareService.WorkWithRequestTask(JsonSerializer.Deserialize<Link>(message));
            
            _logger.LogInformation("{Link} response code {Code}", link.Url, link.HttpStatusCode);

            await UpdateLinkRequestAsync(link, new Uri(_appConfiguration.ApiUri, "link"));

            _logger.LogInformation("{Link} was updated", link.Url);
        });
    }

    private async Task UpdateLinkRequestAsync(Link link, Uri endpointUri)
    {
        using HttpContent content = new StringContent(JsonSerializer.Serialize(link), Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await _httpClient.PutAsync(endpointUri, content).ConfigureAwait(false);

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