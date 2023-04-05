using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HexMaster.UrlShortner.Api.Controllers
{
    [ApiController]
    public class ResolverController : ControllerBase
    {

        [HttpGet]
        [Route("{shortCode}")]
        public async Task<IActionResult> ResolveAsync(string shortCode)
        {
            Console.WriteLine(shortCode);
            var returnObject = new
            {
                ShortCode = shortCode
            };
            return Ok(returnObject);
        }

    }
}
