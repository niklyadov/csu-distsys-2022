using TestApp.DAL;

namespace TestApp;

public record LinkRecord : Entity
{
    public string Url { get; set; }
    public int? HttpStatusCode { get; set; } = null;
    public DateTime? CreatedAt { get; set; } = null;
    public DateTime? ExecutedAt { get; set; } = null;
}