using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;
using PersonaBackend.Models.HandOfZeus;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter("HandOfZeus")]
    public class HandOfZeusController : ControllerBase
    {
        [HttpPost("givePersonasSickness")]
        public async Task<IActionResult> GivePersonasSickness([FromBody] SicknessRequest request)
        {
            return Ok($"Sickness with ID {request.SicknessId} given to Persona/s with ID/s: {string.Join(",", request.PersonaIds)}");
        }

        [HttpPost("givePersonasChild")]
        public async Task<IActionResult> GivePersonasChild([FromBody] ChildRequest request)
        {
            return Ok($"Child persona given to Persona with ID: {string.Join(",", request.PersonaIds)}");
        }

        [HttpDelete("killPersonas")]
        public async Task<IActionResult> KillPersonas([FromBody] KillRequest request)
        {
            return Ok($"Personas with IDs {string.Join(",", request.PersonaIds)} killed");
        }

        [HttpPost("getMarried")]
        public async Task<IActionResult> GetMarried([FromBody] MarriageRequest request)
        {
            return Ok($"Personas with IDs {request.PersonaId1} and {request.PersonaId2} are now married");
        }
    }
}