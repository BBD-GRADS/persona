namespace PersonaBackend.Models.Stocks
{
    public class BuyStockResponse
    {
        public string? ReferenceId { get; set; }
        public double AmountToPay { get; set; }
        public int Quantity { get; set; }
    }
}