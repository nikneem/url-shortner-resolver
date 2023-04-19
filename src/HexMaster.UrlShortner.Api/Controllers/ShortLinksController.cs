using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HexMaster.UrlShortner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShortLinksController : ControllerBase
{
    private readonly IShortLinksService _shortLinksService;


    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? query)
    {
        var ownerId = Guid.NewGuid();
        var responseObject = await _shortLinksService.ListAsync(ownerId, query);
        return Ok(responseObject);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Get(ShortLinkCreateDto dto)
    {
        var responsObject = new
        {
            ResponseCode = "OK",
            SearchQuery = dto
        };
        return Ok(responsObject);
    }

    public ShortLinksController(IShortLinksService shortLinksService)
    {
        _shortLinksService = shortLinksService;
    }
}
