using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using RepositoryLayer.Hashing;
using RepositoryLayer.Helper;
using RepositoryLayer.Entity;
using Microsoft.EntityFrameworkCore;

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
            var role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;
            var user = _userRepository.CreateUser(model.UserName, model.Email, hashedPassword, role);

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
        public string GenerateResetToken(int userId, string email)
        {
            return _jwtHelper.GenerateResetToken(userId, email);
        }
        public UserEntity GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }
        public UserEntity ResetPassword(string token, ResetPasswordRequest model)
        {
            int userId = _jwtHelper.ResetPassword(token, model);
            var user = _userRepository.GetUserById(userId);
            if (user != null)
            {
                string hashedPassword = HashingHelper.HashPassword(model.NewPassword);
                user.PasswordHash = hashedPassword;
                if (_userRepository.UpdateUserPassword(user.Email, user.PasswordHash))
                {
                    return user;
                }
            }
            return null;
        }
        public UserEntity GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }
    }
}
