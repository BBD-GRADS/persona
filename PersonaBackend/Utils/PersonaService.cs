using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Models.Persona;

namespace PersonaBackend.Utils
{
    public class PersonaService
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;

        public PersonaService(Context dbContext, Chronos chronos)
        {
            _dbContext = dbContext;
            _chronos = chronos;
        }

        public void BuyItems(Persona persona)
        {
            try
            {
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

                int numberOfElectronicsToAdd = 3;
                persona.NumElectronicsOwned += numberOfElectronicsToAdd;

                _dbContext.Personas.Update(persona);
            }
            catch (Exception ex)
            {
                //throw;
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

                        _dbContext.FoodItems.Update(foodItem);
                    }
                }
            }
            catch (Exception ex)
            {
                // throw new Exception($"Error updating food storage for persona {persona.Id}: {ex.Message}", ex);
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
                    _dbContext.Personas.Update(persona);
                    _dbContext.FoodItems.Update(healthiestFood);
                }
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

                    _dbContext.EventsOccurred.Add(eventOccurred);
                    _dbContext.Personas.Update(persona); // Update persona in DbContext
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating persona to adult: {ex.Message}", ex);
            }
        }
    }
}