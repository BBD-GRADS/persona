namespace PersonaBackend.Models.Stocks
{
    public class BuyStockRequestByQuantity
    {
        public string? BuyerId { get; set; }
        public string? BusinessId { get; set; }
        public int Quantity { get; set; }
    }
}
