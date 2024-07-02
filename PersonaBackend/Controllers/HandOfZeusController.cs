using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Authentication;
using PersonaBackend.Data;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona.PersonaRequests;
using PersonaBackend.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiKeyAuthFilter("HandOfZeus")]
    public class HandOfZeusController : ControllerBase
    {
        private readonly Context _dbContext;

        public HandOfZeusController(Context dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("startSimulation")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Simulation started successfully", typeof(ApiResponse<bool>))]
        public IActionResult StartSimulation([FromBody] StartSimulationRequest request)
        {
            // Your simulation logic here
            return Ok(new ApiResponse<bool> { Data = true, Message = "Simulation started successfully" });
        }

        [HttpPost("givePersonasSickness")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Sickness given to personas successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> GivePersonasSickness([FromBody] PersonaIdList request)
        {
            try
            {
                var personas = await _dbContext.Personas
                                              .Where(p => request.PersonaIds.Contains(p.Id))
                                              .ToListAsync();

                foreach (var persona in personas)
                {
                    persona.Sick = true;
                    _dbContext.Personas.Update(persona);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse<bool> { Data = true, Message = $"Sickness given to personas with IDs: {string.Join(",", request.PersonaIds)}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool> { Data = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("givePersonasChild")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Child assigned to parent personas successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> GivePersonasChild([FromBody] ParentChildList request)
        {
            try
            {
                var childPersonaIds = request.ParentChildren.Select(pc => pc.ChildId).ToList();
                var parentPersonas = await _dbContext.Personas
                    .Where(p => request.ParentChildren.Any(pc => pc.ParentId == p.Id) || childPersonaIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var parentChildPair in request.ParentChildren)
                {
                    var childPersona = await _dbContext.Personas.FindAsync(parentChildPair.ChildId);
                    if (childPersona != null)
                    {
                        childPersona.ParentId = parentChildPair.ParentId;
                        _dbContext.Personas.Update(childPersona);
                    }
                    else
                    {
                        return NotFound($"Persona with ID {parentChildPair.ChildId} not found");
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse<bool> { Data = true, Message = $"Child persona assigned to Parent persona/s with ID/s: {string.Join(",", request.ParentChildren.Select(pc => pc.ParentId))}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool> { Data = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("killPersonas")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Personas killed successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> KillPersonas([FromBody] PersonaIdList request)
        {
            try
            {
                var personas = await _dbContext.Personas
                                              .Where(p => request.PersonaIds.Contains(p.Id))
                                              .ToListAsync();

                foreach (var persona in personas)
                {
                    persona.Alive = false;
                    _dbContext.Personas.Update(persona);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse<bool> { Data = true, Message = $"Personas with IDs {string.Join(",", request.PersonaIds)} killed" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool> { Data = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("marryPersonas")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Personas married successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> MarryPersonas([FromBody] PersonaMarriageList request)
        {
            try
            {
                if (request.MarriagePairs == null || !request.MarriagePairs.Any())
                {
                    return BadRequest("MarriagePairs list cannot be null or empty.");
                }

                var personaIds = request.MarriagePairs
                                     .SelectMany(pair => new[] { pair.FirstPerson, pair.SecondPerson })
                                     .Distinct()
                                     .ToList();

                var personas = await _dbContext.Personas
                                              .Where(p => personaIds.Contains(p.Id))
                                              .ToListAsync();

                foreach (var pair in request.MarriagePairs)
                {
                    var firstPerson = personas.FirstOrDefault(p => p.Id == pair.FirstPerson);
                    var secondPerson = personas.FirstOrDefault(p => p.Id == pair.SecondPerson);

                    if (firstPerson != null && secondPerson != null)
                    {
                        firstPerson.PartnerId = secondPerson.Id;
                        secondPerson.PartnerId = firstPerson.Id;

                        _dbContext.Personas.Update(firstPerson);
                        _dbContext.Personas.Update(secondPerson);
                    }
                    else
                    {
                        return NotFound($"One or more personas in the marriage pair not found.");
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse<bool> { Data = true, Message = $"Married {request.MarriagePairs.Count} pairs of personas." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool> { Data = false, Message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
