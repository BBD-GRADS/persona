using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;
using PersonaBackend.Models.LifeInsurance;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter("LifeInsurance")]
    public class LifeInsuranceController : ControllerBase
    {
        [HttpPost("InsurePersona")]
        public async Task<IActionResult> InsurePersona([FromBody] LifeInsureRequest request)
        {
            return Ok($"Insurance of type {request.InsuranceTypeId} ensured for Persona with ID {request.PersonaId}");
        }
    }
}