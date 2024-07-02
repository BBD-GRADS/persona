using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class FoodItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("food_date_bought", TypeName = "varchar(10)")]
        public string FoodDateBought { get; set; }

        [Column("food_stored_in_electronic")]
        public bool FoodStoredInElectronic { get; set; }

        [Column("food_health")]
        public int FoodHealth { get; set; }

        [Column("food_eaten")]
        public bool Eaten { get; set; }

        [Column("persona_id")]
        public long PersonaId { get; set; }

        [ForeignKey("PersonaId")]
        public virtual Persona Persona { get; set; }
    }
}