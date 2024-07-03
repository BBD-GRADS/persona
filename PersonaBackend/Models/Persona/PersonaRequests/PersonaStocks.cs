namespace PersonaBackend.Models.Persona.PersonaRequests
{
    public class PersonaStocks
    {
        public long PersonaId { get; set; }
        public List<StockItem>? Stocks { get; set; }
    }
}
