using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PersonaBackend.Data;
using PersonaBackend.Models.Persona;

namespace PersonaBackend.Utils
{
    public class CustomerAccount
    {
        [JsonPropertyName("balanceInMibiBBDough")]
        public decimal BalanceInMibiBBDough { get; set; }
    }

    public class PersonaService
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;
        private readonly HttpClient _httpClient;

        public PersonaService(Context dbContext, Chronos chronos, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _chronos = chronos;
            _httpClient = httpClient;
        }

        public async void BuyItems(Persona persona)
        {
            try
            {
                //TODO: get bank balance

                // var personaId = new { persona.Id };
                // var response = await _httpClient.GetAsync($"https://api.retailbank.projects.bbdgrad.com/api/customers/{personaId}/accounts");
                // // var response = _httpClient.GetAsync($"https://api.retailbank.projects.bbdgrad.com/api/customers/{personaId}/accounts").Result;
                // if (!response.IsSuccessStatusCode)
                // {
                //     return; // Oh no!
                // }

                // var content = await response.Content.ReadAsStringAsync();
                // var options = new JsonSerializerOptions
                // {
                //     PropertyNameCaseInsensitive = true,
                // };

                // var customerAccounts = System.Text.Json.JsonSerializer.Deserialize<List<CustomerAccount>>(content, options);
                // var balance = customerAccounts?.FirstOrDefault()?.BalanceInMibiBBDough ?? 0;

                //talk to retailer
                int numberOfFoodItemsToAdd = 1;
                List<FoodItem> foodItemsToAdd = new List<FoodItem>();

                var requestData = new { consumerId = persona.Id };
                var response = await _httpClient.GetAsync($"https://api.sustenance.projects.bbdgrad.com/api/Buy?consumerId={persona.Id}");
                // if (!response.IsSuccessStatusCode)
                // {
                   // return; // StatusCode((int)response.StatusCode, new ApiResponse<bool> { Data = false, Message = "Failed to create persona accounts at the retail bank." });
                // }

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

                    foodItemsToAdd.Add(foodItem);
                }

                _dbContext.FoodItems.AddRange(foodItemsToAdd);

                //TODO: Calc left over money buy electronics
                //talk to retailer

                int numberOfElectronicsToAdd = 2;
                var requestElectronicsData = new { customerId = persona.Id, quantity = numberOfElectronicsToAdd };
                var electronicsJson = JsonConvert.SerializeObject(requestElectronicsData);
                var electronicsContent = new StringContent(electronicsJson, Encoding.UTF8, "application/json");
                // var responseElectronics = await _httpClient.PostAsync("https://service.electronics.projects.bbdgrad.com/store/order", electronicsContent);
                // if (!responseElectronics.IsSuccessStatusCode)
                // {
                   // return; // StatusCode((int)response.StatusCode, new ApiResponse<bool> { Data = false, Message = "Failed to create persona accounts at the retail bank." });
                // }

                persona.NumElectronicsOwned += numberOfElectronicsToAdd;

                _dbContext.Personas.Update(persona);

                //_dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                // throw; // Uncomment if you want to rethrow the exception
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
                            foodItem.FoodStoredInElectronic = true;
                            foodItemsStoredInElectronic++;
                        }
                        else
                        {
                            foodItem.FoodStoredInElectronic = false;
                        }
                    }
                    _dbContext.FoodItems.UpdateRange(persona.FoodInventory);

                    //_dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // throw new Exception($"Error updating food storage for persona {persona.Id}: {ex.Message}", ex);
            }
        }

        public bool CheckIfDie(Persona persona)
        {
            var died = false;
            try
            {
                if ((persona.Sick || persona.DaysStarving >= 2) && persona.Alive)
                {
                    died = true;
                    persona.Alive = false;

                    var eventOccurred = new EventOccurred
                    {
                        PersonaId1 = persona.Id,
                        PersonaId2 = persona.Id,
                        EventId = (int)EventTypeEnum.Died,
                        DateOccurred = _chronos.GetCurrentDateString()
                    };

                    _dbContext.EventsOccurred.Add(eventOccurred);
                    _dbContext.Personas.Update(persona);
                    //_dbContext.SaveChanges();
                }
                return died;
            }
            catch (Exception ex)
            {
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
                    persona.DaysStarving = 0;
                    _dbContext.FoodItems.Update(healthiestFood);
                }
                else
                {
                    persona.DaysStarving++;
                }
                _dbContext.Personas.Update(persona);
                //_dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
            }
        }

        public void UpdateToAdult(Persona persona)
        {
            try
            {
                var ageInDays = _chronos.getAge(persona.BirthFormatTime);

                if (ageInDays >= 6 * 30)
                {
                    persona.Adult = true;

                    var eventOccurred = new EventOccurred
                    {
                        PersonaId1 = persona.Id,
                        PersonaId2 = persona.Id,
                        EventId = (int)EventTypeEnum.Adult,
                        DateOccurred = _chronos.GetCurrentDateString()
                    };

                    _dbContext.EventsOccurred.Add(eventOccurred);
                    _dbContext.Personas.Update(persona); // Update persona in DbContext
                                                         // _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating persona to adult: {ex.Message}", ex);
            }
        }
    }
}