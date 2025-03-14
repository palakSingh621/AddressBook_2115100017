using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRepository;

        public UserBL(IUserRL userRepository)
        {
            _userRepository = userRepository;
        }

        public ActionResult<string> RegisterUser(RegisterRequest model)
        {
            if (_userRepository.UserExists(model.Email))
                return new ActionResult<string> { Success = false, Message = "Email already exists" };

            string hashedPassword = model.Password;
            _userRepository.CreateUser(model.UserName, model.Email, hashedPassword);

            return new ActionResult<string> { Success = true, Message = "User registered successfully" };
        }

        public ActionResult<string> LoginUser(LoginRequest model)
        {
            var user = _userRepository.GetUserByEmail(model.Email);
            if (user == null || model.Password != user.PasswordHash)
                return new ActionResult<string> { Success = false, Message = "Invalid credentials" };

            return new ActionResult<string> { Success = true, Message = "Login successful", Data = "JWT_TOKEN_HERE" };
        }

        public ActionResult<string> ForgotPassword(ForgetPasswordRequest model)
        {
            // Logic to send a password reset link via email
            return new ActionResult<string> { Success = true, Message = "Password reset link sent" };
        }

        public ActionResult<string> ResetPassword(ResetPasswordRequest model)
        {
            string hashedPassword =model.NewPassword;
            _userRepository.UpdateUserPassword(model.Email, hashedPassword);

            return new ActionResult<string> { Success = true, Message = "Password reset successful" };
        }
    }
}
