using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;
using PersonaBackend.Models.ShortTermLoad.PersonaBackend.Models;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter("ShortTermLoan")]
    public class ShortTermLoanController : ControllerBase
    {
        [HttpPost("LendMoney")]
        public async Task<IActionResult> LendMoney([FromBody] LoanRequest request)
        {
            return Ok($"Loan of {request.LoanAmount} granted to Persona with ID {request.PersonaId} for {request.LoanPeriod} months at {request.Interest}% interest");
        }
    }
}