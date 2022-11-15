using Microsoft.Extensions.Options;

namespace TestApp;

public class MarkAppNameMiddleware : IMiddleware
{
    private readonly AppConfiguration _appConfiguration;

    public MarkAppNameMiddleware(IOptions<AppConfiguration> appConfiguration)
    {
        _appConfiguration = appConfiguration.Value;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Add("API-Name", _appConfiguration.AppName);
        
        await next.Invoke(context);
    }
}