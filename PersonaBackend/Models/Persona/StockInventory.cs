using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class StockInventory
    {
        [Key]
        public int Id { get; set; }

        public int BusinessId { get; set; }

        [ForeignKey("BusinessId")]
        public virtual Business Business { get; set; }

        public int NumStocks { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string DateBought { get; set; }
    }
}
