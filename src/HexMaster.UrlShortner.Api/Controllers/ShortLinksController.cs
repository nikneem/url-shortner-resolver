using HexMaster.UrlShortner.Api.Base;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HexMaster.UrlShortner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShortLinksController : AuthenticatedControllerBase
{
    private readonly IShortLinksService _shortLinksService;

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> List([FromQuery] string? query, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var responseObject = await _shortLinksService.ListAsync(ownerId, query, cancellationToken: token);
        return Ok(responseObject);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Get(Guid id, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var detailsModel = await _shortLinksService.GetAsync(ownerId, id, token);
        return Ok(detailsModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(ShortLinkCreateDto dto, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var responseObject = await _shortLinksService.PostAsync(ownerId, dto.Endpoint, token);
        return Ok(responseObject);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Put(Guid id, ShortLinkDetailsDto details, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var success = await _shortLinksService.PutAsync(ownerId, id, details, token);
        if (success)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<ShortLinkDetailsDto> patchDocument, CancellationToken token)
    {
        var ownerId = GetSubjectId();
        var detailsModel = await _shortLinksService.GetAsync(ownerId, id, token);
        patchDocument.ApplyTo(detailsModel, ModelState);
        var success = await _shortLinksService.PutAsync(ownerId, id, detailsModel, token);
        if (success)
        {
            return Ok();
        }

        return BadRequest();
    }

    public ShortLinksController(IShortLinksService shortLinksService)
    {
        _shortLinksService = shortLinksService;
    }
}
