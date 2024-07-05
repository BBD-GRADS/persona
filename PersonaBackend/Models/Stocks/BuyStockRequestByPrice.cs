namespace PersonaBackend.Models.Stocks
{
    public class BuyStockRequestByPrice
    {
        public string BuyerId { get; set; }
        public string BusinessId { get; set; }
        public double MaxPrice { get; set; }
    }
}
