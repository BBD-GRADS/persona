using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonaBackend.Authentication;
using PersonaBackend.Data;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter("HandOfZeus")]
    public class HandOfZeusController : ControllerBase
    {
        private readonly Context _dbContext;

        public HandOfZeusController(Context dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("givePersonasSickness")]
        public async Task<IActionResult> GivePersonasSickness([FromBody] SicknessRequest request)
        {
            try
            {
                foreach (var personaId in request.PersonaIds)
                {
                    var persona = await _dbContext.Personas.FindAsync(personaId);
                    if (persona != null)
                    {
                        // Assuming SicknessId should be parsed as int
                        int sicknessId;
                        if (int.TryParse(request.SicknessId, out sicknessId))
                        {
                            if (persona.disease_ids == null)
                                persona.disease_ids = new int[] { sicknessId };
                            else
                                persona.disease_ids = persona.disease_ids.Concat(new int[] { sicknessId }).ToArray();

                            _dbContext.Personas.Update(persona);
                        }
                        else
                        {
                            return BadRequest("Invalid SicknessId format");
                        }
                    }
                    else
                    {
                        return NotFound($"Persona with ID {personaId} not found");
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok($"Sickness with ID {request.SicknessId} given to Persona/s with ID/s: {string.Join(",", request.PersonaIds)}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("givePersonasChild")]
        public async Task<IActionResult> GivePersonasChild([FromBody] ChildRequest request)
        {
            try
            {
                foreach (var personaId in request.PersonaIds)
                {
                    var childPersona = await _dbContext.Personas.FindAsync(personaId);
                    if (childPersona != null)
                    {
                        childPersona.parent_id = request.ParentPersonaId;
                        _dbContext.Personas.Update(childPersona);
                    }
                    else
                    {
                        return NotFound($"Persona with ID {personaId} not found");
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok($"Child persona given to Persona with ID: {string.Join(",", request.PersonaIds)}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("killPersonas")]
        public async Task<IActionResult> KillPersonas([FromBody] KillRequest request)
        {
            try
            {
                foreach (var personaId in request.PersonaIds)
                {
                    var persona = await _dbContext.Personas.FindAsync(personaId);
                    if (persona != null)
                    {
                        persona.alive = false;
                        _dbContext.Personas.Update(persona);
                    }
                    else
                    {
                        return NotFound($"Persona with ID {personaId} not found");
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok($"Personas with IDs {string.Join(",", request.PersonaIds)} killed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("getMarried")]
        public async Task<IActionResult> GetMarried([FromBody] MarriageRequest request)
        {
            try
            {
                var persona1 = await _dbContext.Personas.FindAsync(request.PersonaId1);
                var persona2 = await _dbContext.Personas.FindAsync(request.PersonaId2);

                if (persona1 != null && persona2 != null)
                {
                    persona1.partner_id = persona2.Id;
                    persona2.partner_id = persona1.Id;

                    _dbContext.Personas.Update(persona1);
                    _dbContext.Personas.Update(persona2);

                    await _dbContext.SaveChangesAsync();

                    return Ok($"Personas with IDs {request.PersonaId1} and {request.PersonaId2} are now married");
                }
                else
                {
                    return NotFound("One or both personas not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
