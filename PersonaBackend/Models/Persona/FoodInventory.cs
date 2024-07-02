using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class FoodInventory
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string FoodDateBought { get; set; }

        public bool FoodStoredInElectronic { get; set; }

        public int FoodHealth { get; set; }

        public bool Eaten { get; set; }
    }
}
