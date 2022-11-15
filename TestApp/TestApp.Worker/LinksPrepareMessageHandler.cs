using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using StackExchange.Redis;

namespace TestApp.Worker;

public class LinksPrepareMessageHandler : IMessageHandler
{
    private readonly ILogger<LinksPrepareMessageHandler> _logger;
    private readonly IDatabase _cache;
    private readonly HttpClient _httpClient = new ();
    private readonly AppConfiguration _config;

    public LinksPrepareMessageHandler(ILogger<LinksPrepareMessageHandler> logger, IDatabase cache, IOptions<AppConfiguration> config)
    {
        _logger = logger;
        _cache = cache;
        _config = config.Value;
        
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

            if (_cache.StringGet(link.Url).HasValue && int.TryParse(_cache.StringGet(link.Url), out int httpStatusCode))
            {
                link.HttpStatusCode = httpStatusCode;
                _logger.LogInformation("{Link} from cache!", link.Url);
            }
            else
            {
                var response = await _httpClient.GetAsync(link.Url);

                link.HttpStatusCode = (int)response.StatusCode;
                link.ExecutedAt = DateTime.UtcNow;

                _logger.LogInformation("{Link} response code {Code}", link.Url, response.StatusCode);

                _cache.StringSet(link.Url, link.HttpStatusCode.ToString());
                _cache.KeyExpire(link.Url, DateTime.UtcNow.Add(
                    TimeSpan.FromMilliseconds(_config.CacheLifetimeInMilliSeconds)));
            }

            _logger.LogInformation("{Link}", link);
            await UpdateLinkRequestAsync(link, new Uri(_config.ApiUri, "link"));

        }
        catch (Exception exception)
        {
            _logger.LogError("{Exception}", exception);
        }
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