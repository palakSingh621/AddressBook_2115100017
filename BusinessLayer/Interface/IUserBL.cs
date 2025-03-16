using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        ResponseModel<string> RegisterUser(RegisterRequest model);
        ResponseModel<string> LoginUser(LoginRequest model);
        string GenerateResetToken(int userId, string email);
        UserEntity GetUserByEmail(string email);
        UserEntity ResetPassword(string token, ResetPasswordRequest model);
        UserEntity GetUserById(int userId);
    }
}
