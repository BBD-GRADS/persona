using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PersonaBackend.Models.Responses;
using PersonaBackend.Models.Stocks;

namespace PersonaBackend.Utils
{
    public class StockService
    {
        private readonly HttpClient _httpClient;

        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://api.mese.projects.bbdgrad.com");
        }

        public async Task<ApiResponse<StockListing>> SellStockAsync(SellStockRequest request)
        {
            var apiResponse = new ApiResponse<StockListing>();

            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/stocks/sell", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                apiResponse.Data = JsonSerializer.Deserialize<StockListing>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiResponse.Success = true;
                apiResponse.Message = "Stock successfully listed for sale.";
            }
            catch (HttpRequestException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "Failed to list stock for sale.";
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "An error occurred while processing the request.";
            }

            return apiResponse;
        }

        public async Task<ApiResponse<BuyStockResponse>> BuyStockByQuantityAsync(BuyStockRequestByQuantity request)
        {
            var apiResponse = new ApiResponse<BuyStockResponse>();

            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/stocks/buy", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                apiResponse.Data = JsonSerializer.Deserialize<BuyStockResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiResponse.Success = true;
                apiResponse.Message = "Stock purchase request successful.";
            }
            catch (HttpRequestException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "Failed to process stock purchase request.";
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "An error occurred while processing the request.";
            }

            return apiResponse;
        }

        public async Task<ApiResponse<BuyStockResponse>> BuyStockByPriceAsync(BuyStockRequestByPrice request)
        {
            var apiResponse = new ApiResponse<BuyStockResponse>();

            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/stocks/buy", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                apiResponse.Data = JsonSerializer.Deserialize<BuyStockResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiResponse.Success = true;
                apiResponse.Message = "Stock purchase request successful.";
            }
            catch (HttpRequestException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "Failed to process stock purchase request.";
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "An error occurred while processing the request.";
            }

            return apiResponse;
        }

        public async Task<ApiResponse<BusinessMarketValue[]>> GetBusinessesAsync()
        {
            var apiResponse = new ApiResponse<BusinessMarketValue[]>();

            try
            {
                var response = await _httpClient.GetAsync("/businesses");
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<ApiResponse<BusinessMarketValue[]>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data != null && data.Data != null)
                {
                    apiResponse.Data = data.Data;
                    apiResponse.Success = true;
                    apiResponse.Message = "Successfully retrieved businesses.";
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "No businesses found or empty response.";
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HttpRequestException: " + ex.Message);

                apiResponse.Success = false;
                apiResponse.Message = "Failed to retrieve businesses: " + ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);

                apiResponse.Success = false;
                apiResponse.Message = "An error occurred while processing the request.";
            }

            return apiResponse;
        }
    }
}
