using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        ResponseModel<string> RegisterUser(RegisterRequest model);
        ResponseModel<string> LoginUser(LoginRequest model);
        ResponseModel<string> ForgotPassword(ForgetPasswordRequest model);
        ResponseModel<string> ResetPassword(ResetPasswordRequest model);
    }
}
