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
                //TODO: get bank balance
                //talk to retailer
                int numberOfFoodItemsToAdd = 2;//3
                List<FoodItem> foodItemsToAdd = new List<FoodItem>();

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

                int numberOfElectronicsToAdd = 1;
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