﻿using System.Diagnostics;
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
                int numberOfFoodItemsToAdd = 3;
                List<FoodItem> foodItemsToAdd = new List<FoodItem>();

                var requestData = new { consumerId = persona.Id };
                // var response = await _httpClient.GetAsync($"https://api.sustenance.projects.bbdgrad.com/api/Buy?consumerId={persona.Id}");
                //var response = await _httpClient.GetAsync($"https://api.sustenance.projects.bbdgrad.com/api/Buy?consumerId={persona.Id}");
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
                //var requestElectronicsData = new { customerId = persona.Id, quantity = numberOfElectronicsToAdd };
                //var electronicsJson = JsonConvert.SerializeObject(requestElectronicsData);
                //var electronicsContent = new StringContent(electronicsJson, Encoding.UTF8, "application/json");
                //var responseElectronics = await _httpClient.PostAsync("https://service.electronics.projects.bbdgrad.com/store/order", electronicsContent);
                //// if (!responseElectronics.IsSuccessStatusCode)
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
            
            Debug.WriteLine("In update food storage");
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
                    var eventOccurred = new EventOccurred
                    {
                        PersonaId1 = persona.Id,
                        PersonaId2 = (long)persona.NextOfKinId,
                        EventId = (int)EventTypeEnum.Died,
                        DateOccurred = _chronos.GetCurrentDateString()
                    };

                    died = true;

                    _dbContext.EventsOccurred.Add(eventOccurred);
                    _dbContext.Personas.Update(persona);
                    //_dbContext.SaveChanges();
                }
                return died;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

        public long GetNextOfKin(Persona persona, List<Persona> personas)
        {
            if (persona.NextOfKinId != null)
            {
                var currentNextOfKin = personas.FirstOrDefault(p => p.Id == persona.NextOfKinId);
                if (currentNextOfKin != null && currentNextOfKin.Alive)
                {
                    return (long)persona.NextOfKinId;
                }
            }

            var children = personas
                .Where(p => p.ParentId == persona.Id && p.Alive)
                .Select(p => p.Id)
                .ToList();

            if (children.Any())
            {
                return children.First();
            }
            else if (persona.PartnerId != null)
            {
                var partner = personas.FirstOrDefault(p => p.Id == persona.PartnerId && p.Alive);
                if (partner != null)
                {
                    return (long)persona.PartnerId;
                }
            }
            else if (persona.ParentId != null)
            {
                var parent = personas.FirstOrDefault(p => p.Id == persona.ParentId && p.Alive);
                if (parent != null)
                {
                    return (long)persona.ParentId;
                }
            }

            var randomNextOfKin = personas
                .Where(p => p.Id != persona.Id && p.Alive)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefault()?.Id;

            if (randomNextOfKin.HasValue)
            {
                return randomNextOfKin.Value;
            }

            return 0;
        }

        public void UpdateToAdult(Persona persona)
        {
            try
            {
                var ageInDays = _chronos.getAge(persona.BirthFormatTime);

                if (ageInDays >= 6 * 30)
                {
                    persona.Adult = true;

                    Random random = new Random();
                    int random_choice = random.Next(1, 3);
                    int random_capacity = random.Next(1, 9);
                    if (random_choice == 1)
                    {
                        var requestBuyHouseData = new { buyerId = persona.Id, numUnits = random_capacity };
                        var requestBuyHouseJson = JsonConvert.SerializeObject(requestBuyHouseData);
                        var houseContent = new StringContent(requestBuyHouseJson, Encoding.UTF8, "application/json");
                        // var housePost = _httpClient.PostAsync("https://api.sales.projects.bbdgrad.com/api/buy", houseContent);

                    }
                    else if (random_choice == 2)
                    {
                        var requestRentHouseData = new { buyerId = persona.Id, numUnits = random_capacity };
                        var requestRentHouseJson = JsonConvert.SerializeObject(requestRentHouseData);
                        var houseContent = new StringContent(requestRentHouseJson, Encoding.UTF8, "application/json");
                        // var rentPost = _httpClient.PostAsync("https://api.rentals.projects.bbdgrad.com/api/rentals", houseContent);
                    }




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

        public void sendSickPersonToHealthcare(Persona persona)
        {
            try
            {

                var requestData = new { personaId = persona.Id };
                var requestJson = JsonConvert.SerializeObject(requestData);
                var sickPersonaIdContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                // var housePost = _httpClient.PostAsync("https://api.care.projects.bbdgrad.com/api/patient", sickPersonaIdContent);

                persona.Sick = false;
                _dbContext.Personas.Update(persona); // Update persona in DbContext
                                                     // _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating persona to adult: {ex.Message}", ex);
            }
        }

        public async Task updateHouseOwningStatusAsync(long personaId, int HomeOwningStatusId)
        {
            var alivePersonas = await _dbContext.Personas
                    .Where(p => p.Id == personaId)
                    .ToListAsync();

            foreach (var persona in alivePersonas) // workaround
            {
                persona.HomeOwningStatusId = HomeOwningStatusId;
                _dbContext.Personas.Update(persona);
            }
        }
    }
}