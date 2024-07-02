using PersonaBackend.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class Persona
    {
        [Key]
        public long id { get; set; }

        public long? NextOfKinId { get; set; }

        [ForeignKey("NextOfKinId")]
        public virtual Persona NextOfKin { get; set; }

        public long? PartnerId { get; set; }

        [ForeignKey("PartnerId")]
        public virtual Persona Partner { get; set; }

        public long? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Persona Parent { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string BirthFormatTime { get; set; }

        public int Hunger { get; set; }
        public int Health { get; set; }
        public bool Alive { get; set; }
        public bool Sick { get; set; }
        public int NumElectronicsOwned { get; set; }

        public int HomeOwningStatusId { get; set; }

        [ForeignKey("HomeOwningStatusId")]
        public virtual HomeOwningStatus HomeOwningStatus { get; set; }

        public int FoodInventoryId { get; set; }

        [ForeignKey("FoodInventoryId")]
        public virtual FoodInventory FoodInventory { get; set; }

        public int StockInventoryId { get; set; }

        [ForeignKey("StockInventoryId")]
        public virtual StockInventory StockInventory { get; set; }
    }
}