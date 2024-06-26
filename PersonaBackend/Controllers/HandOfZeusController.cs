using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonaBackend.Authentication;
using PersonaBackend.Data;
using PersonaBackend.Models.examples;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
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

        [HttpPost("startSimulation")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseBoolExample))]
        public async Task<IActionResult> StartSimulation([FromBody] StartSimulationRequest request)
        {
            return Ok();
        }

        [HttpPost("givePersonasSickness")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseBoolExample))]
        public async Task<IActionResult> GivePersonasSickness([FromBody] PersonaIdList request)
        {
            try
            {
                //foreach (var personaId in request.PersonaIds)
                //{
                //    var persona = await _dbContext.Personas.FindAsync(personaId);
                //    if (persona != null)
                //    {
                //    }
                //    else
                //    {
                //        return NotFound($"Persona with ID {personaId} not found");
                //    }
                //}

                await _dbContext.SaveChangesAsync();

                return Ok($"Sickness given to Persona/s with ID/s: {string.Join(",", request.PersonaIds)}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("givePersonasChild")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseBoolExample))]
        public async Task<IActionResult> GivePersonasChild([FromBody] PersonaIdList request)
        {
            try
            {
                foreach (var personaId in request.PersonaIds)
                {
                    var childPersona = await _dbContext.Personas.FindAsync(personaId);
                    if (childPersona != null)
                    {
                        // childPersona.parent_id = request.ParentPersonaId;
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

        [HttpPost("killPersonas")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseBoolExample))]
        public async Task<IActionResult> KillPersonas([FromBody] PersonaIdList request)
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

        [HttpPost("marryPersonas")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseBoolExample))]
        public async Task<IActionResult> MarryPersonas([FromBody] PersonaPairs request)
        {
            return Ok();
        }
    }
}