using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Utils;
using System.Diagnostics;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodItemsController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly Chronos _chronos;

        public FoodItemsController(Context dbContext, Chronos chronos)
        {
            _dbContext = dbContext;

            _chronos = chronos;
        }

        [HttpDelete("removeOldFood")]
        public async Task<IActionResult> removeOldFood()
        {
            try
            {
                var eatenFoodItems = await _dbContext.FoodItems
                    .Where(f => f.Eaten || f.FoodHealth == 0)
                    .ToListAsync();
                _dbContext.FoodItems.RemoveRange(eatenFoodItems);

                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("updateAllFood")]
        public async Task<IActionResult> updateAllFood()
        {
            Debug.WriteLine("Update food");
            try
            {
                var foodItems = await _dbContext.FoodItems
                    .Where(f => !f.Eaten && f.FoodHealth > 0)
                    .ToListAsync();

                foreach (var foodItem in foodItems)
                {
                    var ageInDays = _chronos.getAge(foodItem.FoodDateBought);

                    Debug.WriteLine(ageInDays);

                    var maxDaysBeforeExpiry = foodItem.FoodStoredInElectronic ? 5 : 3;
                    var healthDecreasePerDay = 100 / maxDaysBeforeExpiry;


                    Debug.WriteLine(healthDecreasePerDay);

                    foodItem.FoodHealth = Math.Max(0, 100 - (ageInDays * healthDecreasePerDay));

                    _dbContext.FoodItems.Update(foodItem);
                }

                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}