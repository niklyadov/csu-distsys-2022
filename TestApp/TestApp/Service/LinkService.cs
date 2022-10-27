using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using TestApp.DAL;
using TestApp.DAL.QueryBuilders;

namespace TestApp.Service;

public class LinkService
{
    private readonly ApplicationContext _context;
    private readonly IProducingService _producingService;
    private LinksQueryBuilder _queryBuilder => new (_context);
    
    public LinkService(ApplicationContext context, IProducingService producingService)
    {
        _context = context;
        _producingService = producingService;
    }
    
    public async Task<ActionResult<LinkRecord>> AddLinkRecord(string url)
    {
        var linkRecord = new LinkRecord
        {
            Url = url,
            CreatedAt = DateTime.UtcNow
        };
        
        var addedLink = await _queryBuilder.AddAsync(linkRecord);
        
        await _producingService.SendAsync(addedLink, "test-app", "links-prepare");

        return addedLink;
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