using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonaBackend.Authentication;
using PersonaBackend.Data;
using PersonaBackend.Models.examples;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Responses;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        /// <summary>
        /// Gets persona's info
        /// </summary>
        /// <param name="personaId">ID number of persona</param>
        /// <remarks>Some response persona data may be redacted depending on your API key</remarks>
        /// <returns>Personas info</returns>
        // GET: api/persona/{id}
        [HttpGet("{personaId}")]
        [ProducesResponseType(typeof(ApiResponse<Persona>), 200)]
        [ApiKeyAuthFilter("")] //TODO ADD services
        public async Task<IActionResult> GetPersonaById(string personaId)
        {
            //maybe redact data based on API key
            return Ok();
        }

        /// <summary>
        /// Retrieves all alive personas.
        /// </summary>
        [HttpGet("getAlivePersonas")]
        [ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetAlivePersonas()
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves all childless personas.
        /// </summary>
        [HttpGet("getChildlessPersonas")]
        [ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetChildlessPersonas()
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves all unmarried personas.
        /// </summary>
        [HttpGet("getSinglePersonas")]
        [ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetSinglePersonas()
        {
            return Ok();
        }

        /// <summary>
        /// Gets list of personas that died between dates
        /// </summary>
        /// <param name="startDate">Start date in format YY|MM|dd (e.g., "22|06|24")</param>
        /// <param name="endDate">End date in format YY|MM|dd (defaults to startDate if not provided)</param>
        /// <returns>List of personas who died between the specified dates</returns>
        [HttpGet("hasDied")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        [ApiKeyAuthFilter("HandOfZeus")]
        public async Task<IActionResult> HasDied(string startDate, string endDate = null)
        {
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = startDate;
            }
            return Ok();
        }

        /// <summary>
        /// Gets list of personas that became an adult between dates
        /// </summary>
        /// <param name="startDate">Start date in format YY|MM|dd (e.g., "22|06|24")</param>
        /// <param name="endDate">End date in format YY|MM|dd (defaults to startDate if not provided)</param>
        /// <returns>List of personas who died between the specified dates</returns>
        [HttpGet("becameAdult")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        [ApiKeyAuthFilter("HandOfZeus")]
        public async Task<IActionResult> BecameAdult(string startDate, string endDate = null)
        {
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = startDate;
            }
            return Ok();
        }

        /// <summary>
        /// Gets list of personas that got married between dates
        /// </summary>
        /// <param name="startDate">Start date in format YY|MM|dd (e.g., "22|06|24")</param>
        /// <param name="endDate">End date in format YY|MM|dd (defaults to startDate if not provided)</param>
        /// <returns>List of personas who died between the specified dates</returns>
        [HttpGet("gotMarried")]
        [ProducesResponseType(typeof(ApiResponse<PersonaPairs>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseMarriedPairExample))]
        [ApiKeyAuthFilter("HandOfZeus")]
        public async Task<IActionResult> GotMarried(string startDate, string endDate = null)
        {
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = startDate;
            }
            return Ok();
        }

        /// <summary>
        /// Gets list of personas that had a child between dates
        /// </summary>
        /// <param name="startDate">Start date in format YY|MM|dd (e.g., "22|06|24")</param>
        /// <param name="endDate">End date in format YY|MM|dd (defaults to startDate if not provided)</param>
        /// <returns>List of personas who died between the specified dates</returns>
        [HttpGet("hadChild")]
        [ProducesResponseType(typeof(ApiResponse<ParentChildList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseChildPairExample))]
        [ApiKeyAuthFilter("HandOfZeus")]
        public async Task<IActionResult> HadChild(string startDate, string endDate = null)
        {
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = startDate;
            }
            return Ok();
        }
    }
}