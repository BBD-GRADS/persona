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

        [HttpPost("startSimulation")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Simulation started successfully", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> StartSimulation([FromBody] StartSimulationRequest request)
        {
            try
            {
                await DeleteAllDataAsync();

                //TODO check  request data VALIDATE
                _chronos.SetSimulationStartDate(request.StartDate);
                await _awsManagerService.PutParameterAsync("/simulation/date", request.StartDate);

                if (request.NumberOfPersonas < 1 || request.NumberOfPersonas > 50000)
                {
                    return BadRequest(new ApiResponse<bool> { Data = false, Message = "Invalid request. The number of personas must be between 1 and 50,000." });
                }

                var personas = new List<Persona>();
                var events = new List<EventOccurred>();
                var personaIDs = new List<long>();

                for (int i = 0; i < request.NumberOfPersonas; i++)
                {
                    var persona = new Persona
                    {
                        NextOfKinId = null,
                        PartnerId = null,
                        ParentId = null,
                        BirthFormatTime = request.StartDate,
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
                //var requestData = new { PersonaIds = personaIDs };
                //var json = JsonConvert.SerializeObject(requestData);
                //var content = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await _httpClient.PostAsync("https://api.retailbank.projects.bbdgrad.com/api.customers", content);
                //if (!response.IsSuccessStatusCode)
                //{
                //    return StatusCode((int)response.StatusCode, new ApiResponse<bool> { Data = false, Message = "Failed to create persona accounts at the retail bank." });
                //}

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
                        DateOccurred = request.StartDate
                    };

                    _dbContext.EventsOccurred.Add(eventOccurred);
                }

                await _dbContext.SaveChangesAsync();

                await _awsManagerService.EnableSchedule("sim-schedule", true);
                return Ok(new ApiResponse<bool> { Success = true, Data = true, Message = $"Simulation started successfully with {request.NumberOfPersonas} persona records" });
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