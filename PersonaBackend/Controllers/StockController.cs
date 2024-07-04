using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Models.Responses;
using PersonaBackend.Models.Stocks;
using PersonaBackend.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockService _stockService;

        public StockController(StockService stockService)
        {
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }

        private IActionResult HandleException(Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        [HttpPost("SellStock")]
        [ProducesResponseType(typeof(ApiResponse<StockListing>), 200)]
        [SwaggerResponse(200, "Stock successfully listed for sale.", typeof(ApiResponse<StockListing>))]
        [SwaggerResponse(500, "An error occurred while processing the request.", typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SellStockAsync([FromBody] SellStockRequest request)
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
