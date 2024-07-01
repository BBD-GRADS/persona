using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class StockInventory
    {
        [Key]
        public int Id {  get; set; }
        public int business_id { get; set; }
        public int num_stocks { get; set; }
    }
}
