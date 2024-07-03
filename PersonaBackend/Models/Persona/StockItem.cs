using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona.PersonaRequests;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class StockItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("persona_id")]
        public long PersonaId { get; set; }

        [ForeignKey("PersonaId")]
        public virtual Persona Persona { get; set; }

        [Column("business_id")]
        public int BusinessId { get; set; }

        [ForeignKey("BusinessId")]
        public virtual Business Business { get; set; }

        [Column("num_stocks")]
        public int NumStocks { get; set; }

        [Column("date_bought", TypeName = "varchar(10)")]
        public string DateBought { get; set; }

        public static implicit operator StockItem(PersonaStocks v)
        {
            throw new NotImplementedException();
        }
    }
}