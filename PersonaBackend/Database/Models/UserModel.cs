using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Database.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
