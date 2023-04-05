using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HexMaster.UrlShortner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortLinks : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery]string? query)
        {
            var responsObject = new
            {
                ResponseCode = "OK",
                SearchQuery = query
            };
            return Ok(responsObject);
        }

    }
}
