using PersonaBackend.Database.Models;

namespace PersonaBackend.Database.IRepositories
{
    public interface IUserRepository
    {
        ICollection<UserModel> GetUsers();
        UserModel GetUser(int userId);
        bool UserExists(int userId);
        bool CreateUser(UserModel users);
        bool UpdateUser(UserModel users);
        bool DeleteUser(UserModel users);
        bool Save(); 
    }
}
