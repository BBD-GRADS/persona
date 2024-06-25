using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;
using PersonaBackend.Models.HealthInsurance;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter("HealthInsurance")]
    public class HealthInsuranceController : ControllerBase
    {
        [HttpPost("InsurePersona")]
        public async Task<IActionResult> InsurePersona([FromBody] HealthInsureRequest request)
        {
            return Ok($"Insurance of type {request.InsuranceTypeId} ensured for Persona with ID {request.PersonaId}");
        }
    }
}