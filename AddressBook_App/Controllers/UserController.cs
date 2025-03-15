using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBook_App.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger, IUserBL userService)
        {
            _logger = logger;
            _userService = userService;
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
        public IActionResult ForgotPassword([FromBody] ForgetPasswordRequest model)
        {
            var response = _userService.ForgotPassword(model);
            return Ok(response);
        }
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <returns>Password Updated successfully</returns>
        [HttpPost]
        [Route("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var response = _userService.ResetPassword(model);
            return Ok(response);
        }
    }
}
