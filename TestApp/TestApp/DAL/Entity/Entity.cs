using System.Diagnostics.CodeAnalysis;

namespace TestApp.DAL;

public interface IEntity
{
    public long Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDateTime { get; set; }
}

public abstract record Entity : IEntity
{
    public long Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDateTime { get; set; }
}