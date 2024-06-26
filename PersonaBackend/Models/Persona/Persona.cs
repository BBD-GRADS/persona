using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class Persona
    {
        [Key]
        public int Id { get; set; }
        public string? firstNames { get; set; }
        public string? lastNames { get; set; }
        public int age_in_days { get; set; }
        public int hunger { get; set; }
        public bool alive { get; set; }
        public int health_from_food { get; set; }
        public int num_food_items { get; set; }
        public int num_electronics { get; set; }
        public int stock_inventory_id { get; set; }
        public int[]? disease_ids { get; set;}
        public int next_of_kin_id { get; set; }
        public int partner_id { get; set; }
        public int parent_id { get; set; }
        public int home_owning_status_id { get; set; }
        public int bank_account_id { get; set; }
        public int short_term_lending_account_id { get; set; }
        public int mortgage_account_id {  get; set; }
    }
}
