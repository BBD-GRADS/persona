namespace PersonaBackend.Models.Stocks.StockRequests
{
    public class SellStockRequestNew
    {
        public long PersonaId { get; set; }
        public int BusinessId { get; set; }
        public int Quantity { get; set; }
    }
}
