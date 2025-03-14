using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        bool UserExists(string email);
        void CreateUser(string username, string email, string passwordHash);
        UserEntity GetUserByEmail(string email);
        void UpdateUserPassword(string email, string newPasswordHash);
    }
}
