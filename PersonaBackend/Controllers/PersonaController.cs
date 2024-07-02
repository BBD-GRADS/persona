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
using System.Buffers.Text;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public void BuyItems(Persona persona)
        {
            try
            {
                // TODO: Implement API calls to bank and retailer
                // Buy # depending on money ?
                int numberOfFoodItemsToAdd = 3;

                for (int i = 0; i < numberOfFoodItemsToAdd; i++)
                {
                    var foodItem = new FoodItem
                    {
                        PersonaId = persona.Id,
                        Eaten = false,
                        FoodDateBought = _chronos.GetCurrentDateString(),
                        FoodStoredInElectronic = false,
                        FoodHealth = 100,
                    };

                    _dbContext.FoodItems.Add(foodItem);
                }

                // Calculate money left after buying food items TODO
                // Buy electronics
                int numberOfElectronicsToAdd = 3;
                persona.NumElectronicsOwned += numberOfElectronicsToAdd;

                // Update persona in DbContext
                _dbContext.Personas.Update(persona);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdatePersonaFoodStorage(Persona persona)
        {
            try
            {
                int numElectronicsOwned = persona.NumElectronicsOwned;
                int foodItemsStoredInElectronic = persona.FoodInventory.Count(f => f.FoodStoredInElectronic);

                if (persona.FoodInventory != null && persona.FoodInventory.Any())
                {
                    foreach (var foodItem in persona.FoodInventory)
                    {
                        if (foodItem.FoodStoredInElectronic)
                        {
                            continue; // Skip already stored items
                        }

                        if (foodItemsStoredInElectronic < numElectronicsOwned)
                        {
                            // Store food item in electronics
                            foodItem.FoodStoredInElectronic = true;
                            foodItemsStoredInElectronic++;
                        }
                        else
                        {
                            // Store food item normally
                            foodItem.FoodStoredInElectronic = false;
                        }

                        _dbContext.FoodItems.Update(foodItem);
                    }

                    // Save changes to the database
                    //await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                throw new Exception($"Error updating food storage for persona {persona.Id}: {ex.Message}", ex);
            }
        }

        public bool CheckHealthStatus(Persona persona)
        {
            var died = false;
            try
            {
                if ((persona.Sick || persona.DaysStarving >= 2) && persona.Alive)
                {
                    died = true;
                    persona.Alive = false; // Mark persona as deceased

                    var eventOccurred = new EventOccurred
                    {
                        PersonaId1 = persona.Id,
                        PersonaId2 = persona.Id,
                        EventId = (int)EventTypeEnum.Died,
                        DateOccurred = _chronos.GetCurrentDateString()
                    };

                    _dbContext.EventsOccurred.Add(eventOccurred);

                    // Optionally: Call other APIs or perform additional actions for a deceased persona

                    _dbContext.Personas.Update(persona);

                    // await _dbContext.SaveChangesAsync();
                }
                return died;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                return died;
            }
        }

        public void EatFood(Persona persona)
        {
            try
            {
                var healthiestFood = persona.FoodInventory
                    .Where(f => !f.Eaten)
                    .OrderByDescending(f => f.FoodHealth)
                    .FirstOrDefault();

                if (healthiestFood != null)
                {
                    int hungerAfterEating = (int)Math.Round(persona.Hunger * 0.25);

                    healthiestFood.Eaten = true;
                    _dbContext.Personas.Update(persona);
                    _dbContext.FoodItems.Update(healthiestFood);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
            }
        }

        [HttpPost("updatePersonaEvent")] //2
        public async Task<IActionResult> updatePersonaEvent() //ran each simulation event
        {
            try
            {
                var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Alive && p.Adult)
                    .Include(p => p.FoodInventory)
                    .ToListAsync();

                foreach (var persona in alivePersonas)
                {
                    var died = CheckHealthStatus(persona);
                    if (died)
                    {
                        continue;
                    }
                    persona.Hunger = 100;
                    BuyItems(persona);
                    EatFood(persona);
                    UpdatePersonaFoodStorage(persona);
                }

                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

                    //if (ageInDays >= 6 * 30) TODO replace
                    if (ageInDays >= 2)
                    {
                        persona.Adult = true;

                        var eventOccurred = new EventOccurred
                        {
                            PersonaId1 = persona.Id,
                            PersonaId2 = persona.Id,
                            EventId = (int)EventTypeEnum.Adult,
                            DateOccurred = _chronos.GetCurrentDateString()
                        };

                        // Save event to database
                        _dbContext.EventsOccurred.Add(eventOccurred);
                    }
                }

                await _dbContext.SaveChangesAsync();

                // TODO: Call needed APIs

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
    }
}