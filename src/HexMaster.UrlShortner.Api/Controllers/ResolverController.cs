﻿using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace HexMaster.UrlShortner.Api.Controllers
{
    [ApiController]
    public class ResolverController : ControllerBase
    {
        private readonly IShortLinksService _service;
        private readonly ILogger<ResolverController> _logger;
        private readonly IConfiguration _configuration;

        private const string DefaultRedirectUrl = "https://app.tinylnk.nl";

        [HttpGet]
        [Route("")]
        [Route("{shortCode}")]
        public async Task<IActionResult> ResolveAsync(string? shortCode)
        {
            var defaultRedirectUrl = _configuration.GetValue<string>("FrontEndEndpoint") ?? DefaultRedirectUrl;
            try
            {
                if (!string.IsNullOrEmpty(shortCode))
                {
                    var result = await _service.ResolveAsync(shortCode);
                    defaultRedirectUrl = result.EndpointUrl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve short code {shortCode}", shortCode);
            }
            return Redirect(defaultRedirectUrl);
        }

        public ResolverController(IShortLinksService service, ILogger<ResolverController> logger, IConfiguration configuration)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

    }
}
