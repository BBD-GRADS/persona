using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Models.Persona;
using PersonaBackend.Models.Responses;
using PersonaBackend.Models.Stocks;
using PersonaBackend.Models.Stocks.StockRequests;
using PersonaBackend.Utils;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly Context _context;
        private readonly StockService _stockService;

        public StockController(Context context, StockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        [HttpPost("BuyStock")]
        [ProducesResponseType(typeof(ApiResponse<StockItem>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BuyStock([FromBody] BuyStockRequest request)
        {
            try
            {
                var persona = await _context.Personas
                    .Include(p => p.StockInventory)
                    .FirstOrDefaultAsync(p => p.Id == request.PersonaId);

                if (persona == null)
                {
                    return NotFound("Persona not found.");
                }

                var businessesResponse = await _stockService.GetBusinessesAsync();
                if (!businessesResponse.Success || businessesResponse.Data == null || businessesResponse.Data.Length == 0)
                {
                    return NotFound("No businesses found.");
                }

                var business = businessesResponse.Data.FirstOrDefault(b => b.Id.Equals(request.BusinessId));

                if (business == null)
                {
                    return NotFound("Business not found.");
                }

                decimal totalCost = (decimal)(request.Quantity * business.CurrentMarketValue);

                if (persona.NumElectronicsOwned < totalCost)
                {
                    return BadRequest("Persona does not have enough funds.");
                }

                var stockItem = persona.StockInventory.FirstOrDefault(si => si.BusinessId == request.BusinessId);

                if (stockItem != null)
                {
                    stockItem.NumStocks += request.Quantity;
                }
                else
                {
                    stockItem = new StockItem
                    {
                        PersonaId = persona.Id,
                        BusinessId = request.BusinessId,
                        NumStocks = request.Quantity,
                        DateBought = DateTime.Now.ToString("yyyy-MM-dd")
                    };

                    _context.StockItems.Add(stockItem);
                    persona.StockInventory.Add(stockItem);
                }

                await _context.SaveChangesAsync();

                var apiResponse = new ApiResponse<StockItem>
                {
                    Success = true,
                    Message = "Stock purchase successful.",
                    Data = stockItem
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("SellStock")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SellStock([FromBody] Models.Stocks.StockRequests.SellStockRequest request)
        {
            try
            {
                var persona = await _context.Personas
                    .Include(p => p.StockInventory)
                    .FirstOrDefaultAsync(p => p.Id == request.PersonaId);

                if (persona == null)
                {
                    return NotFound("Persona not found.");
                }

                var stockItem = persona.StockInventory.FirstOrDefault(si => si.BusinessId == request.BusinessId);

                if (stockItem == null || stockItem.NumStocks < request.Quantity)
                {
                    return BadRequest("Persona does not have enough stocks to sell.");
                }

                var businessesResponse = await _stockService.GetBusinessesAsync();
                if (!businessesResponse.Success || businessesResponse.Data == null || businessesResponse.Data.Length == 0)
                {
                    return NotFound("No businesses found.");
                }

                var business = businessesResponse.Data.FirstOrDefault(b => b.Id.Equals(request.BusinessId));

                if (business == null)
                {
                    return NotFound("Business not found.");
                }

                stockItem.NumStocks -= request.Quantity;

                await _context.SaveChangesAsync();

                var apiResponse = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Stock sale successful.",
                    Data = true
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
