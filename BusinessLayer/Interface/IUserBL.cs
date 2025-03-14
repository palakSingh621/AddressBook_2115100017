using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        ActionResult<string> RegisterUser(RegisterRequest model);
        ActionResult<string> LoginUser(LoginRequest model);
        ActionResult<string> ForgotPassword(ForgetPasswordRequest model);
        ActionResult<string> ResetPassword(ResetPasswordRequest model);
    }
}
