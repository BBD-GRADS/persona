using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Models.examples;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Persona.PersonaRequests;
using PersonaBackend.Models.Responses;
using PersonaBackend.Utils;
using Swashbuckle.AspNetCore.Filters;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;
        private readonly PersonaService _personaService;

        public PersonaController(Context dbContext, Chronos chronos)
        {
            _dbContext = dbContext;
            _chronos = chronos;
            _personaService = new PersonaService(dbContext, chronos);
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        [HttpPost("updatePersonaEvent")] //2
        public async Task<IActionResult> updatePersonaEvent()
        {
            try
            {
                var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Alive)
                    .Include(p => p.FoodInventory)
                    .ToListAsync();

                foreach (var persona in alivePersonas)
                {
                    var died = _personaService.CheckIfDie(persona);
                    if (died)
                    {
                        continue;
                    }

                    if (!persona.Adult)
                    {
                        _personaService.UpdateToAdult(persona);
                        continue;
                    }

                    persona.Hunger = 100;
                    _personaService.UpdatePersonaFoodStorage(persona);
                    _personaService.EatFood(persona);
                    _personaService.BuyItems(persona);
                }
                _dbContext.UpdateRange(alivePersonas);
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
        [ProducesResponseType(typeof(ApiResponse<PersonaList>), 200)]
        [SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetAlivePersonas()
        {
            try
            {
                var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Alive)
                    .ToListAsync();

                var response = new ApiResponse<PersonaList>
                {
                    Success = true,
                    Message = "Alive personas retrieved",
                    Data = new PersonaList { personas = alivePersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all persons, dead and alive.
        /// </summary>
        [HttpGet("getAllPersonas")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaList>), 200)]
        //[SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetAllPersonas()
        {
            try
            {
                var allPersonas = await _dbContext.Personas.ToListAsync();

                var response = new ApiResponse<PersonaList>
                {
                    Success = true,
                    Message = "List of all Personas",
                    Data = new PersonaList { personas = allPersonas }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all persons, dead and alive.
        /// </summary>
        [HttpGet("getAllPersonaID")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        //[SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetAllPersonasID()
        {
            try
            {
                var allPersonas = await _dbContext.Personas.Select(s => s.Id).ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "List of all Personas",
                    Data = new PersonaIdList { PersonaIds = allPersonas }
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
        //[SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetChildlessPersonas()
        {
            try
            {
                var childlessPersonas = await _dbContext.Personas
                    .Where(p => p.ParentId == null).Select(s => s.Id).ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Retrieve all childless personas",
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
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        //[ApiKeyAuthFilter("HandOfZeus")]
        public async Task<IActionResult> GetSinglePersonas()
        {
            try
            {
                var singlePersonas = await _dbContext.Personas
                    .Where(p => p.PartnerId == null)
                    .Select(s => s.Id)
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
        /// Count all married personas
        /// </summary>
        [HttpGet("Married")]
        [ProducesResponseType(typeof(ApiResponse<long>), 200)]
        public async Task<IActionResult> GetMarriedPersonas()
        {
            try
            {
                var marriedPersons = await _dbContext.Personas
                    .Where(p => p.PartnerId != null)
                    .ToListAsync();

                var response = new ApiResponse<long>
                {
                    Success = true,
                    Message = "List of Married Persons",
                    Data = marriedPersons.Count()
                };

                return Ok(response);
            }
            catch(Exception ex) 
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Count all birth since the beginning of time
        /// </summary>
        [HttpGet("Births")]
        [ProducesResponseType(typeof(ApiResponse<long>), 200)]
        //[SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetTotalBirths()
        {
            try
            {
                var totalBirths = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Born)
                    .Select(s => s.PersonaId1).ToListAsync();

                var response = new ApiResponse<long>
                {
                    Success = true,
                    Message = "Total Persons born since the beginning of time",
                    Data = totalBirths.Count()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get a list of dead people to date
        /// </summary>
        [HttpGet("DeadPersons")]
        [ProducesResponseType(typeof(ApiResponse<PersonaList>), 200)]
        //[SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> GetDeceasedPersons()
        {
            try
            {
                var deadPersons = await _dbContext.Personas
                    .Where(p => p.Alive == false)
                    .ToListAsync();

                var response = new ApiResponse<PersonaList>
                {
                    Success = true,
                    Message = "List of Dead Persons",
                    Data = new PersonaList { personas = deadPersons }
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
        //[SwaggerResponseExample(200, typeof(ApiResponsePersonaIdListEmptyExample))]
        public async Task<IActionResult> HasDied(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var diedPersonas = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Died && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
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
                    .Where(e => e.EventId == (int)EventTypeEnum.Adult && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
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
        //[SwaggerResponseExample(200, typeof(ApiResponseMarriedPairExample))]
        public async Task<IActionResult> GotMarried(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var marriedPersonas = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Married && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
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
        //[SwaggerResponseExample(200, typeof(ApiResponseChildPairExample))]
        public async Task<IActionResult> HadChild(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var parentChildPairs = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Born && DateTime.Parse(e.DateOccurred) >= DateTime.Parse(startDate) && DateTime.Parse(e.DateOccurred) <= DateTime.Parse(endDate))
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

        /// <summary>
        /// Post parent id to create a new child
        /// </summary>
        [HttpPost("makeNewChild")]
        [ProducesResponseType(typeof(ApiResponse<Persona>), 200)]
        //[SwaggerResponseExample(200, typeof(ApiResponseChildPairExample))]
        public async Task<IActionResult> MakeNewChild(long parent_id)
        {
            try
            {
                var timeNow = DateTime.Now;

                var newChild = new Persona
                {
                    BirthFormatTime = timeNow.ToString(),
                    ParentId = parent_id,
                    Hunger = 0, 
                    Health = 100, 
                    Adult = false,
                    Alive = true, 
                    Sick = false,
                    NumElectronicsOwned = 0, 
                    HomeOwningStatusId = null 
                };

                _dbContext.Personas.Add(newChild);
                await _dbContext.SaveChangesAsync();

                var response = new ApiResponse<Persona>
                {
                    Success = true,
                    Message = "New child persona created successfully",
                    Data = newChild
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get all stocks related to a persona by id
        /// </summary>
        [HttpPost("getPersonaStocks")]
        [ProducesResponseType(typeof(ApiResponse<StockItem>), 200)]
        //[SwaggerResponseExample(200, typeof(ApiResponseChildPairExample))]
        public async Task<IActionResult> GetPersonaStocks(long persona_id)
        {
            try
            {
                var stockItems = await _dbContext.StockItems
                    .Where(s => s.PersonaId == persona_id)
                    .ToListAsync();

                var response = new ApiResponse<StockItem>
                {
                    Success = true,
                    Message = "Stock items retrieved successfully",
                    Data = new PersonaStocks
                    {
                        PersonaId = persona_id,
                        Stocks = stockItems
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