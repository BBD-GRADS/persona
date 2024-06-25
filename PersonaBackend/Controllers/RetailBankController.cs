using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;
using PersonaBackend.Models.RetailBank;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter("RetailBank")]
    public class RetailBankController : ControllerBase
    {
        [HttpPost("OpenAccount")]
        public async Task<IActionResult> OpenAccount([FromBody] OpenBankAccountRequest request)
        {
            return Ok($"Account opened for Persona with ID {request.PersonaId} and Bank Account ID {request.BankAccountId}");
        }
    }
}