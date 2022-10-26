using Microsoft.AspNetCore.Mvc;
using TestApp.DAL;
using TestApp.DAL.QueryBuilders;

namespace TestApp.Service;

public class LinkService
{
    private readonly ApplicationContext _context;
    private LinksQueryBuilder _queryBuilder => new (_context);
    
    public LinkService(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task<ActionResult<LinkRecord>> AddLinkRecord(string url)
    {
        return await _queryBuilder.AddAsync(new LinkRecord
        {
            Url = url,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<ActionResult<LinkRecord>> GetLinkRecord(long id)
    {
        return await _queryBuilder.WithId(id).SingleAsync();
    }

    public async Task<ActionResult<LinkRecord>> UpdateLinkRecord(LinkRecord linkRecord)
    {
        return await _queryBuilder.UpdateAsync(linkRecord);
    }
}