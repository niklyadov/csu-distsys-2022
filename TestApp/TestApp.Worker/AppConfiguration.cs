namespace TestApp.Worker;

public class AppConfiguration
{
    public Uri ApiUri { get; set; } = default!;
    public long CacheLifetimeInMilliSeconds { get; set; } = 30000;
}