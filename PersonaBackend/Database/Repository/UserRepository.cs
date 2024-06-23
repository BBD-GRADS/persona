using PersonaBackend.Database.Models;
using PersonaBackend.Database.IRepositories;

namespace PersonaBackend.Database.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly PersonaDatabaseContext context;

        public UserRepository(PersonaDatabaseContext context)
        {
            this.context = context;
        }
        public bool UserExists(int userId)
        {
            return context.Users.Any(u => u.UserId == userId);
        }

        public bool CreateUser(UserModel user)
        {
            context.Add(user);
            return Save();
        }

        public bool DeleteUser(UserModel user)
        {

            context.Remove(user);
            return Save();
        }

        public ICollection<UserModel> GetUsers()
        {
            return context.Users.ToList();
        }

        public UserModel GetUser(int userId)
        {
            return context.Users.Where(u => u.UserId == userId).FirstOrDefault();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0;
        }

        public bool UpdateUser(UserModel users)
        {
            context.Update(users);
            return Save();
        }
    }
}