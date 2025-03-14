﻿using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBook_App.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailService _emailService;
        public UserController(ILogger<UserController> logger, IUserBL userService, IEmailService emailService)
        {
            _logger = logger;
            _userService = userService;
            _emailService = emailService;
        }
        /// <summary>
        /// Register user
        /// </summary>
        /// <returns>User registered successfully</returns>
        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            try
            {
                var response = _userService.RegisterUser(model);
                if (!response.Success)
                {
                    _logger.LogWarning("User registration failed: Email already exists.");
                    return BadRequest(response);
                }
                _logger.LogInformation("User Register successfully..");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user.");
                return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
            }
        }
        /// <summary>
        /// Login User 
        /// </summary>
        /// <returns>User Login successfully</returns>
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            try
            {
                _logger.LogInformation("Login attemp for user: {0}", model.Email);
                var response = _userService.LoginUser(model);
                if (!response.Success)
                {
                    _logger.LogWarning("Invalid login attempt for user: {0}", model.Email);
                    return Unauthorized(response);
                }
                _logger.LogInformation("User {0} logged in successfully.", model.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return BadRequest(new { Success = false, Message = "Login failed.", Error = ex.Message });
            }
        }
        /// <summary>
        /// Forget Password 
        /// </summary>
        /// <returns>Reset link sent to registered user email</returns>
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordRequest model)
        {
            _logger.LogInformation("ForgotPassword request received with email: {Email}", model.Email);
            var response = new ResponseModel<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return BadRequest(new { success = false, message = "Email is required" });
                }
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null)
                {
                    string token = _userService.GenerateResetToken(user.Id, user.Email);
                    _emailService.SendResetEmail(user.Email, token);
                    response.Success = true;
                    response.Message = "Reset password link sent to email";
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "User not found";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process Failed");
                return BadRequest(new { Success = false, Message = "Process Failed", Error = ex.Message });
            }
        }
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <returns>Password Updated successfully</returns>
        [HttpPost]
        [Route("reset-password")]
        public IActionResult ResetPassword([FromQuery] string token, [FromBody] ResetPasswordRequest model)
        {
            _logger.LogInformation("Resetting Password...");
            var response = new ResponseModel<string>();
            try
            {
                var user = _userService.ResetPassword(token, model);
                if (user != null)
                {
                    response.Success = true;
                    response.Message = "Password reset successful";
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "Invalid or expired token";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process Failed");
                return BadRequest(new { Success = false, Message = "Process Failed", Error = ex.Message });
            }
        }
    }
}
