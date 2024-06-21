using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost("Impregnate")]
        [ServiceAuthorization("HandOfZeus")]
        public async Task<IActionResult> Impregnate()
        {
            return Ok("Impregnated");
        }

        [HttpPost("GiveMoney")]
        [ServiceAuthorization("RetailBank")]
        public async Task<IActionResult> GiveMoney()
        {
            return Ok("you got money");
        }

        [HttpGet("Test")]
        [ServiceAuthorization("RetailBank", "HandOfZeus")]
        public async Task<IActionResult> Test()
        {
            return Ok("test");
        }

        [HttpGet("NoAuth")]
        public async Task<IActionResult> NoAuth()
        {
            return Ok("NoAuth");
        }
    }
}