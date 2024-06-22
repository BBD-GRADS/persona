using Microsoft.EntityFrameworkCore;
using PersonaBackend.Database.Models;

namespace PersonaBackend.Database
{
    public class PersonaDatabaseContext: DbContext
    {
        public PersonaDatabaseContext(DbContextOptions<PersonaDatabaseContext> options) : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; } = default!;
    }
}
