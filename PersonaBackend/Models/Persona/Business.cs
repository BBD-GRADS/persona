using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class Business
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("business_name")]
        public string BusinessName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("business_type")]
        public string BusinessType { get; set; }
    }
}