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
using Swashbuckle.AspNetCore.Annotations;

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
            catch (Exception ex) {
                return HandleException(ex);
            }
        }
        
        [HttpPost("SellStock")]
        [ProducesResponseType(typeof(ApiResponse<StockListing>), 200)]
        [SwaggerResponse(200, "Stock successfully listed for sale.", typeof(ApiResponse<StockListing>))]
        [SwaggerResponse(500, "An error occurred while processing the request.", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SellStockAsync([FromBody] Models.Stocks.SellStockRequest request)
        {
            try
            {
                var response = await _stockService.SellStockAsync(request);
                return Ok(response); 
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("BuyStockByQuantity")]
        [ProducesResponseType(typeof(ApiResponse<BuyStockResponse>), 200)]
        [SwaggerResponse(200, "Stock purchase request successful.", typeof(ApiResponse<BuyStockResponse>))]
        [SwaggerResponse(500, "An error occurred while processing the request.", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> BuyStockByQuantityAsync([FromBody] BuyStockRequestByQuantity request)
        {
            try
            {
                var response = await _stockService.BuyStockByQuantityAsync(request);
                return Ok(response); 
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("BuyStockByPrice")]
        [ProducesResponseType(typeof(ApiResponse<BuyStockResponse>), 200)]
        [SwaggerResponse(200, "Stock purchase request successful.", typeof(ApiResponse<BuyStockResponse>))]
        [SwaggerResponse(500, "An error occurred while processing the request.", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> BuyStockByPriceAsync([FromBody] BuyStockRequestByPrice request)
        {
            try
            {
                var response = await _stockService.BuyStockByPriceAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetAllBusinesses")]
        [ProducesResponseType(typeof(ApiResponse<BusinessMarketValue[]>), 200)]
        [SwaggerResponse(200, "Successfully retrieved businesses.", typeof(ApiResponse<BusinessMarketValue[]>))]
        [SwaggerResponse(500, "An error occurred while processing the request.", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> GetAllBusinesses()
        {
            try
            {
                var response = await _stockService.GetBusinessesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
