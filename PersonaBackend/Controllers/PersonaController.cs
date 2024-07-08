using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Models.examples;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Persona.PersonaRequests;
using PersonaBackend.Models.Responses;
using PersonaBackend.Utils;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics;
using System.Globalization;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;
        private readonly HttpClient _httpClient;
        private readonly PersonaService _personaService;
        private readonly AWSManagerService _awsManagerService;

        public PersonaController(Context dbContext, Chronos chronos, HttpClient httpClient, AWSManagerService aWSManagerService)
        {
            _dbContext = dbContext;
            _chronos = chronos;
            _personaService = new PersonaService(dbContext, chronos, httpClient);
            _awsManagerService = aWSManagerService;
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        [HttpPost("updatePersonaEvent")] //2
        public async Task<IActionResult> updatePersonaEvent()
        {
            Debug.WriteLine("Starting update");
            try
            {
                await _awsManagerService.PutParameterAsync("/simulation/date", _chronos.GetCurrentDateString());

                Debug.WriteLine("Getting alives:");
                var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Alive)
                    .Include(p => p.FoodInventory)
                    .ToListAsync();

                Debug.WriteLine(" for eaching");
                foreach (var persona in alivePersonas)
                {
                    persona.NextOfKinId = _personaService.GetNextOfKin(persona, alivePersonas);

                    var died = _personaService.CheckIfDie(persona);
                    if (died)
                    {
                        persona.Alive = false;
                        continue;
                    }

                    if (!persona.Adult)
                    {
                        _personaService.UpdateToAdult(persona);
                        continue;
                    }

                    persona.Hunger = 100;
                    
                    Debug.WriteLine("Done next of kin, check if dead and adult update");
                    _personaService.UpdatePersonaFoodStorage(persona);
                    _personaService.EatFood(persona);
                    // buy item
                    _personaService.BuyItems(persona);
                    Debug.WriteLine("Finished food storage update, eating food and buying items");

                    if (persona.Sick)
                    {
                        _personaService.sendSickPersonToHealthcare(persona);
                    }
                }
                
                Debug.WriteLine("End of foreach");
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
        /// Retrieves all persons, dead and alive.
        /// </summary>
        [HttpGet("getAlivePersonasIDs")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        public async Task<IActionResult> getAlivePersonasIDs()
        {
            try
            {
                var allPersonas = await _dbContext.Personas.Where(p => p.Alive.Equals(true)).Select(s => s.Id).ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "List of all Personas alive",
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
        /// /// <summary>
        /// Retrieves all childless personas.
        /// </summary>
        [HttpGet("getAlivePersonaIds")]
        //[ApiKeyAuthFilter("HandOfZeus")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
        public async Task<IActionResult> GetAlivePersonaIds()
        {
            try
            {
                var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Alive == true).Select(s => s.Id).ToListAsync();

                var response = new ApiResponse<PersonaIdList>
                {
                    Success = true,
                    Message = "Retrieve all alive persona ids",
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
        /// Retrieves all unmarried personas.
        /// </summary>
        [HttpGet("getSinglePersonas")]
        [ProducesResponseType(typeof(ApiResponse<PersonaIdList>), 200)]
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
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Count all birth since the beginning of time
        /// </summary>
        [HttpGet("Births")]
        [ProducesResponseType(typeof(ApiResponse<long>), 200)]
        public async Task<IActionResult> GetTotalBirths()
        {
            try
            {
                var totalBirths = await _dbContext.Personas
                    .Select(s => s.BirthFormatTime != null).ToListAsync();

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
        public async Task<IActionResult> HasDied(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var events = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Died)
                    .ToListAsync();

                var diedPersonas = events
                  .Where(e => _chronos.CompareDates(e.DateOccurred, startDate) >= 0 && _chronos.CompareDates(e.DateOccurred, endDate) <= 0)
                  .Select(e => new DeathPair
                  {
                      deceased = e.PersonaId1,
                      next_of_kin = e.PersonaId2
                  })
                  .ToList();

                var response = new ApiResponse<DeathPairList>
                {
                    Success = true,
                    Message = "Died personas retrieved",
                    Data = new DeathPairList
                    {
                        DeathPairs = diedPersonas.ToList()
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

                var events = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Adult)
                    .ToListAsync();

                var adultPersonas = events
                    .Where(e => _chronos.CompareDates(e.DateOccurred, startDate) >= 0 && _chronos.CompareDates(e.DateOccurred, endDate) <= 0)
                    .Select(e => e.PersonaId1)
                    .ToList();

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
        public async Task<IActionResult> GotMarried(string startDate, string endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var events = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Married)
                    .ToListAsync();

                var marriedPersonas = events
                    .Where(e => _chronos.CompareDates(e.DateOccurred, startDate) >= 0 && _chronos.CompareDates(e.DateOccurred, endDate) <= 0)
                    .Select(e => new PersonaMarriagePair
                    {
                        partner_a = e.PersonaId1,
                        partner_b = e.PersonaId2
                    })
                    .ToList();

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
        public async Task<IActionResult> HadChild(string startDate, string endDate = null)
        {
            
            try
            {
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = startDate;
                }

                var events = await _dbContext.EventsOccurred
                    .Where(e => e.EventId == (int)EventTypeEnum.Born)
                    .ToListAsync();

                var parentChildPairs = events
                    .Where(e => _chronos.CompareDates(e.DateOccurred, startDate) >= 0 && _chronos.CompareDates(e.DateOccurred, endDate) <= 0)
                    .Select(e => new ParentChildPair
                    {
                        parent = e.PersonaId1,
                        child = e.PersonaId2
                    })
                    .ToList();

                var response = new ApiResponse<ParentChildList>
                {
                    Success = true,
                    Message = "Parent-child relationships retrieved",
                    Data = new ParentChildList
                    {
                        ParentChildPairs = parentChildPairs.ToList()
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
        /// Creates a new child persona with the specified parent ID.
        /// </summary>
        /// <param name="request">Request object containing parent ID.</param>
        [HttpPost("makeNewChild")]
        [ProducesResponseType(typeof(ApiResponse<Persona>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "New child persona created successfully", typeof(ApiResponse<Persona>))]
        public async Task<IActionResult> MakeNewChild([FromBody] MakeNewChildRequest request)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var newChild = new Persona
                    {
                        BirthFormatTime = _chronos.GetCurrentDateString(),
                        ParentId = request.ParentId,
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

                    var eventOccurred = new EventOccurred
                    {
                        PersonaId1 = request.ParentId,
                        PersonaId2 = newChild.Id,
                        EventId = (int)EventTypeEnum.Born,
                        DateOccurred = _chronos.GetCurrentDateString()
                    };

                    _dbContext.EventsOccurred.Add(eventOccurred);
                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();

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
                    transaction.Rollback();
                    return HandleException(ex);
                }
            }
        }

        /// <summary>
        /// Get all stocks related to a persona by id
        /// </summary>
        [HttpGet("getPersonaStocks")]
        [ProducesResponseType(typeof(ApiResponse<StockItem>), 200)]
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


        [HttpPost("buyHouseSuccess")]
        public async Task<IActionResult> BuyHouseSuccess([FromBody] long personaId, bool isSuccess)
        {
            if (isSuccess)
            {
                await _personaService.updateHouseOwningStatusAsync(personaId, 2);

            }
            else
            {
                // do nothing, home status still homeless
            }
            return Ok();

        }
    }
}