using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using RepositoryLayer.Hashing;
using RepositoryLayer.Helper;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRepository;
        private readonly JwtTokenHelper _jwtHelper;

        public UserBL(IUserRL userRepository,JwtTokenHelper jwtTokenHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtTokenHelper;
        }

        public ResponseModel<String> RegisterUser(RegisterRequest model)
        {
            if (_userRepository.UserExists(model.Email))
                return new ResponseModel<String> 
                { 
                    Success = false,
                    Message = "Email already exists" 
                };

            string hashedPassword = HashingHelper.HashPassword(model.Password);
            var user=_userRepository.CreateUser(model.UserName, model.Email, hashedPassword);

            var response= new ResponseModel<String>
            { 
                Success = true, 
                Message = "User registered successfully",
                Data = $"User Name: {user.UserName}     Email: {user.Email}"
            };
            var token = _jwtHelper.GenerateToken(user);
            return response;
        }

        public ResponseModel<string> LoginUser(LoginRequest model)
        {
            var user = _userRepository.GetUserByEmail(model.Email);
            if (user == null || !HashingHelper.VerifyPassword(model.Password, user.PasswordHash))
                return new ResponseModel<string> 
                { 
                    Success = false,
                    Message = "Invalid credentials"
                };
            string token = _jwtHelper.GenerateToken(user);
            return new ResponseModel<string> 
            { 
                Success = true,
                Message = "Login successful",
                Data = token
            };
        }

        public ResponseModel<string> ForgotPassword(ForgetPasswordRequest model)
        {
            // Logic to send a password reset link via email
            return new ResponseModel<string> { Success = true, Message = "Password reset link sent" };
        }

        public ResponseModel<string> ResetPassword(ResetPasswordRequest model)
        {
            string hashedPassword =model.NewPassword;
            _userRepository.UpdateUserPassword(model.Email, hashedPassword);

            return new ResponseModel<string> { Success = true, Message = "Password reset successful" };
        }
    }
}
