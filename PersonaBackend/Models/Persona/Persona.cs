using PersonaBackend.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class Persona
    {
        [Key]
        public Int64 id { get; set; }

        public string birth_format_time { get; set; }
        public bool alive { get; set; }
        public bool sick { get; set; }
        public int health { get; set; }
        public int hunger { get; set; }
        public int parent_id { get; set; }
        public int partner_id { get; set; }
        public int num_electronics_owned { get; set; }
        public int stock_inventory_id { get; set; }
        public int food_inventory_id { get; set; }
        public int next_of_kin_id { get; set; }
        public int home_owning_status_id { get; set; }
    }
}