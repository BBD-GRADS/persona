using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PersonaBackend.Authentication;
using PersonaBackend.Data;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Persona.PersonaRequests;
using PersonaBackend.Models.Responses;
using PersonaBackend.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandOfZeusController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;
        private readonly AWSManagerService _awsManagerService;
        private readonly HttpClient _httpClient;

        public HandOfZeusController(Context dbContext, Chronos chronos, AWSManagerService awsManagerService, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _chronos = chronos;
            _awsManagerService = awsManagerService;
            _httpClient = httpClient;
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        [HttpPost("startSimulation")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Simulation started successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> StartSimulation([FromBody] StartSimulationRequest request)
        {
            try
            {
                if (request.action == "reset")
                {
                    await _awsManagerService.EnableSchedule("sim-schedule", false);

                    await DeleteAllDataAsync();
                    return Ok(new ApiResponse<bool> { Success = true, Data = true, Message = $"Simulation reset successfully" });
                }

                //TODO check  request data VALIDATE
                _chronos.SetSimulationStartDate(request.startTime);
                var numberOfPersonas = 1000;

                await _awsManagerService.PutParameterAsync("/simulation/date", _chronos.GetCurrentDateString());

                var personas = new List<Persona>();
                var events = new List<EventOccurred>();
                var personaIDs = new List<long>();

                for (int i = 0; i < numberOfPersonas; i++)
                {
                    var persona = new Persona
                    {
                        NextOfKinId = null,
                        PartnerId = null,
                        ParentId = null,
                        BirthFormatTime = _chronos.GetCurrentDateString(),
                        Hunger = 0,
                        Health = 100,
                        Alive = true,
                        Sick = false,
                        Adult = true,
                        NumElectronicsOwned = 0,
                        //HomeOwningStatusId = 0, //default? TODO make enum add a status here later
                    };

                    personas.Add(persona);

                    personaIDs.Add(i);

                    //todoretial bank USE DAILY EVENT INSTEAD
                    //TODO call labour people to get job USE DAILY EVENT INSTEAD
                    //TODO call insure LIFE and HEALTH USE DAILY EVENT INSTEAD
                    //TODO get house rent/buy - see salary?
                }

                // Call retail bank to open account and deposit 1000 starting amount
                var requestData = new { PersonaIds = personaIDs };
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.retailbank.projects.bbdgrad.com/api/customers", content);
                if (!response.IsSuccessStatusCode)
                {
                    // return StatusCode((int)response.StatusCode, new ApiResponse<bool> { Data = false, Message = "Failed to create persona accounts at the retail bank." });
                }

                await _dbContext.Personas.AddRangeAsync(personas);

                await _dbContext.SaveChangesAsync();

                var newPersonasIds = await _dbContext.Personas
                .Select(p => p.Id)
                .ToListAsync();

                foreach (var personaId in newPersonasIds)
                {
                    var eventOccurred = new EventOccurred
                    {
                        PersonaId1 = personaId,
                        PersonaId2 = personaId,
                        EventId = (int)EventTypeEnum.Adult,
                        DateOccurred = _chronos.GetCurrentDateString()
                    };

                    _dbContext.EventsOccurred.Add(eventOccurred);
                }

                await _dbContext.SaveChangesAsync();

                await _awsManagerService.EnableSchedule("sim-schedule", true);
                return Ok(new ApiResponse<bool> { Success = true, Data = true, Message = $"Simulation started successfully with {numberOfPersonas} persona records" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool> { Data = false, Message = "An error occurred while starting the simulation." });
            }
        }

        private async Task DeleteAllDataAsync()
        {
            //_dbContext.StockItems.RemoveRange(_dbContext.StockItems);
            _dbContext.FoodItems.RemoveRange(_dbContext.FoodItems);
            _dbContext.EventsOccurred.RemoveRange(_dbContext.EventsOccurred);
            _dbContext.Personas.RemoveRange(_dbContext.Personas);

            //_dbContext.Businesses.RemoveRange(_dbContext.Businesses);
            //_dbContext.HomeOwningStatuses.RemoveRange(_dbContext.HomeOwningStatuses);
            //_dbContext.EventTypes.RemoveRange(_dbContext.EventTypes);

            await _dbContext.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"Personas_id_seq\" RESTART WITH 1");


            await _dbContext.SaveChangesAsync();
        }

        [HttpPost("givePersonasSickness")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Sickness given to personas successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> GivePersonasSickness([FromBody] PersonaIdList request)
        {
            try
            {
                var personas = await _dbContext.Personas
                                              .Where(p => request.PersonaIds.Contains(p.Id) && p.Alive.Equals(true))
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var parentIds = request.ParentChildPairs?.Select(pair => pair.Parent).ToList();

                    if (parentIds == null || !parentIds.Any())
                    {
                        throw new Exception();
                    }

                    var parentsExist = await _dbContext.Personas
                        .Where(p => parentIds.Contains(p.Id) && p.Alive)
                        .Select(p => p.Id)
                        .ToListAsync();

                    var eventsToAdd = new List<EventOccurred>();

                    foreach (var parentId in parentsExist)
                    {
                        var newChild = new Persona
                        {
                            BirthFormatTime = _chronos.GetCurrentDateString(),
                            ParentId = parentId,
                            Hunger = 0,
                            Health = 100,
                            Adult = false,
                            Alive = true,
                            Sick = false,
                            NumElectronicsOwned = 0,
                            HomeOwningStatusId = null
                        };

                        _dbContext.Personas.Add(newChild);

                        var childId = newChild.Id;

                        var eventOccurred = new EventOccurred
                        {
                            PersonaId1 = parentId,
                            PersonaId2 = childId,
                            EventId = (int)EventTypeEnum.Born,
                            DateOccurred = _chronos.GetCurrentDateString(),
                        };

                        eventsToAdd.Add(eventOccurred);
                    }

                    await _dbContext.SaveChangesAsync();

                    _dbContext.EventsOccurred.AddRange(eventsToAdd);
                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();

                    var response = new ApiResponse<bool>
                    {
                        Success = true,
                        Message = $" ${parentsExist.Count()} Personas birth has been recorded successfully",
                        Data = true,
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

        [HttpPost("killPersonas")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Personas killed successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> KillPersonas([FromBody] PersonaIdList request)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var personas = await _dbContext.Personas
                                                  .Where(p => request.PersonaIds.Contains(p.Id) && p.Alive.Equals(true))
                                                  .ToListAsync();

                    foreach (var persona in personas)
                    {
                        persona.Alive = false;
                        _dbContext.Personas.Update(persona);
                    }

                    var eventsToAdd = personas.Select(persona => new EventOccurred
                    {
                        PersonaId1 = persona.Id,
                        PersonaId2 = persona.Id,
                        EventId = (int)EventTypeEnum.Died,
                        DateOccurred = _chronos.GetCurrentDateString(),
                    }).ToList();

                    _dbContext.EventsOccurred.AddRange(eventsToAdd);
                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();

                    var response = new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "Personas death successfully recorded",
                        Data = true,
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

        [HttpPost("marryPersonas")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Personas married successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> MarryPersonas([FromBody] PersonaMarriageList request)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
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
                                                  .Where(p => personaIds.Contains(p.Id) && p.Alive.Equals(true))
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

                    var eventsToAdd = request.MarriagePairs.Select(pair => new EventOccurred
                    {
                        PersonaId1 = pair.FirstPerson,
                        PersonaId2 = pair.SecondPerson,
                        EventId = (int)EventTypeEnum.Married,
                        DateOccurred = _chronos.GetCurrentDateString()
                    }).ToList();

                    _dbContext.EventsOccurred.AddRange(eventsToAdd);
                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();

                    var response = new ApiResponse<bool>
                    {
                        Success = true,
                        Message = $"${personaIds.Count()} Personas have been married successfully",
                        Data = true,
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
    }
}