using Microsoft.AspNetCore.Mvc;
using TestApp.Service;

namespace TestApp.Controller;

[Route("link")]
public class LinkController : ControllerBase
{
    private readonly LinkService _service;

    public LinkController(LinkService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<ActionResult<LinkRecord>> AddLinkRecord([FromBody] string url)
    {
        return await _service.AddLinkRecord(url);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<LinkRecord>> GetLinkRecord([FromRoute] long id)
    {
        return await _service.GetLinkRecord(id);
    }
    
    [HttpPut]
    public async Task<ActionResult<LinkRecord>> UpdateLinkRecord([FromBody] LinkRecord linkRecord)
    {
        return await _service.UpdateLinkRecord(linkRecord);
    }
}