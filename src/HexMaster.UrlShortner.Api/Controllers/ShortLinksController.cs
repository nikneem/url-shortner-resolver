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
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] string? query, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var responseObject = await _shortLinksService.ListAsync(ownerId, query, cancellationToken: token);
        return Ok(responseObject);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(ShortLinkCreateDto dto, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var responseObject = await _shortLinksService.PostAsync(ownerId, dto.Endpoint, token);
        return Ok(responseObject);
    }

    private string GetSubjectId()
    {
        if (HttpContext.User.Identity != null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var subject = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                if (string.IsNullOrWhiteSpace(subject))
                {
                    if (HttpContext.User.Claims.Any(c =>
                            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
                    {
                        subject = HttpContext.User.Claims.FirstOrDefault(c =>
                            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                    }
                }
                return subject;
            }
        }

        return string.Empty;
    }

    public ShortLinksController(IShortLinksService shortLinksService)
    {
        _shortLinksService = shortLinksService;
    }
}
