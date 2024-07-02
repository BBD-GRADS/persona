using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Authentication;
using PersonaBackend.Data;
using PersonaBackend.Models.examples;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Persona.PersonaRequests;
using PersonaBackend.Models.Responses;
using PersonaBackend.Utils;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;

        public PersonaController(Context dbContext, Chronos chronos)
        {
            _dbContext = dbContext;
            _chronos = chronos;
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        [HttpPost("updateToAdult")]
        public async Task<IActionResult> UpdateToAdult()
        {
            try
            {
                var children = await _dbContext.Personas
                    .Where(p => !p.Adult)
                    .ToListAsync();

                foreach (var persona in children)
                {
                    var ageInDays = _chronos.getAge(persona.BirthFormatTime);

                    //if (ageInDays >= 6 * 30)
                    if (ageInDays >= 2)

                    {
                        persona.Adult = true;

                        var eventOccurred = new EventOccurred
                        {
                            PersonaId1 = persona.Id,
                            EventId = (int)EventTypeEnum.Adult,
                            DateOccurred = _chronos.GetCurrentDateString()
                        };
                        //TODO call needed APIs
                    }
                }
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets persona's info by ID.
        /// </summary>
        [HttpGet("{personaId}")]
        [ProducesResponseType(typeof(ApiResponse<Persona>), 200)]
        public async Task<IActionResult> GetPersonaById(long personaId)
        {
            try
            {
                var persona = await _dbContext.Personas.FirstOrDefaultAsync(p => p.Id == personaId);

                if (persona == null)
                    return NotFound($"Persona with ID {personaId} not found");

                var response = new ApiResponse<Persona>
                {
                    Success = true,
                    Message = "Persona found",
                    Data = persona
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all alive personas.
        /// </summary>
        [HttpGet("getAlivePersonas")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetAlivePersonas()
        {
            try
            {
                var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Alive)
                    .Select(p => p.Id)
                    .ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Alive personas retrieved",
                    Data = new PersonaIdList { PersonaIds = alivePersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all childless personas.
        /// </summary>
        [HttpGet("getChildlessPersonas")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetChildlessPersonas()
        {
            try
            {
                var childlessPersonas = await _dbContext.Personas
                    .Where(p => p.ParentId == null)
                    .Select(p => p.Id)
                    .ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Childless personas retrieved",
                    Data = new PersonaIdList { PersonaIds = childlessPersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all unmarried personas.
        /// </summary>
        [HttpGet("getSinglePersonas")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetSinglePersonas()
        {
            try
            {
                var singlePersonas = await _dbContext.Personas
                    .Where(p => p.PartnerId == null)
                    .Select(p => p.Id)
                    .ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Single personas retrieved",
                    Data = new PersonaIdList { PersonaIds = singlePersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets list of personas that died between dates.
        /// </summary>
        [HttpGet("hasDied")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> HasDied(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var diedPersonas = await _dbContext.EventsOccurred
                    .Where(e => e.EventType.EventName == (int)EventTypeEnum.Died && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
                    .Select(e => e.PersonaId1)
                    .ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Died personas retrieved",
                    Data = new PersonaIdList { PersonaIds = diedPersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets list of personas that became adults between dates.
        /// </summary>
        [HttpGet("becameAdult")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> BecameAdult(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var adultPersonas = await _dbContext.EventsOccurred
                    .Where(e => e.EventType.EventName == (int)EventTypeEnum.Adult && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
                    .Select(e => e.PersonaId1)
                    .ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Adult personas retrieved",
                    Data = new PersonaIdList { PersonaIds = adultPersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets list of personas that got married between dates.
        /// </summary>
        [HttpGet("gotMarried")]
        [ProducesResponseType(typeof(ApiResponse<PersonaMarriageList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseMarriedPairExample))]
        public async Task<IActionResult> GotMarried(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var marriedPersonas = await _dbContext.EventsOccurred
                    .Where(e => e.EventType.EventName == (int)EventTypeEnum.Married && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
                    .Select(e => new PersonaMarriagePair
                    {
                        FirstPerson = e.PersonaId1,
                        SecondPerson = e.PersonaId2
                    })
                    .ToListAsync();

                var response = new ApiResponse<PersonaMarriageList>
                {
                    Success = true,
                    Message = "Married personas retrieved",
                    Data = new PersonaMarriageList
                    {
                        MarriagePairs = marriedPersonas.ToList()
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Gets list of personas that had a child between dates.
        /// </summary>
        [HttpGet("hadChild")]
        [ProducesResponseType(typeof(ApiResponse<ParentChildList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponseChildPairExample))]
        public async Task<IActionResult> HadChild(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var parentChildPairs = await _dbContext.EventsOccurred
                    .Where(e => e.EventType.EventName == (int)EventTypeEnum.Born && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
                    .Select(e => new ParentChildPair
                    {
                        ParentId = e.PersonaId1,
                        ChildId = e.PersonaId2
                    })
                    .ToListAsync();

                var response = new ApiResponse<ParentChildList>
                {
                    Success = true,
                    Message = "Parent-child relationships retrieved",
                    Data = new ParentChildList
                    {
                        ParentChildren = parentChildPairs.ToList()
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}