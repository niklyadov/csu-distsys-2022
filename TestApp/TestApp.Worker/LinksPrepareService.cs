namespace TestApp.Worker;

public class LinksPrepareService
{
    private readonly HttpClient _httpClient = new ();
    public async Task<Link> WorkWithRequestTask(Link? link)
    {
        if (link == null) throw new ArgumentNullException(nameof(link));
             
         var response = await _httpClient.GetAsync(link.Url);

         link.HttpStatusCode = (int)response.StatusCode;
         link.ExecutedAt = DateTime.UtcNow;

         return link;
    }
}