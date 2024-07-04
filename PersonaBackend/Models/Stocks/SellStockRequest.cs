namespace PersonaBackend.Models.Stocks
{
    public class SellStockRequest
    {
        public string? SellerId { get; set; }
        public string? CompanyId { get; set; }
        public int Quantity { get; set; }
    }
}