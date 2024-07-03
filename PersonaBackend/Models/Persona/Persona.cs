using Microsoft.EntityFrameworkCore;
using PersonaBackend.Interfaces;
using PersonaBackend.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class Persona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("next_of_kin_id")]
        public long? NextOfKinId { get; set; }

        [Column("partner_id")]
        public long? PartnerId { get; set; }

        [Column("parent_id")]
        public long? ParentId { get; set; }

        [Column("birth_format_time", TypeName = "varchar(10)")]
        public string BirthFormatTime { get; set; }

        [Column("hunger")]
        public int Hunger { get; set; }

        [Column("health")]
        public int Health { get; set; }

        [Column("days_starving")]
        public int DaysStarving { get; set; }

        [Column("alive")]
        public bool Alive { get; set; }

        [Column("adult")]
        public bool Adult { get; set; }

        [Column("sick")]
        public bool Sick { get; set; }

        [Column("num_electronics_owned")]
        public int NumElectronicsOwned { get; set; }

        [Column("home_owning_status_id")]
        public int? HomeOwningStatusId { get; set; }

        public virtual ICollection<StockItem> StockInventory { get; set; } = new HashSet<StockItem>();

        public virtual ICollection<FoodItem> FoodInventory { get; set; } = new HashSet<FoodItem>();
    }
}