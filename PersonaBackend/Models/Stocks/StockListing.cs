namespace PersonaBackend.Models.Stocks
{
    public class StockListing
    {
        public string? OwnerId { get; set; }
        public string? BusinessId { get; set; }
        public int Quantity { get; set; }
        public double CurrentMarketValue { get; set; }
    }
}