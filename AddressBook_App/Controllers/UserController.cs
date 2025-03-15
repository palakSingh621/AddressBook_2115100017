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

        public UserController(IUserBL userService)
        {
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
            var response = _userService.RegisterUser(model);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        /// <summary>
        /// Login User 
        /// </summary>
        /// <returns>User Login successfully</returns>
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            var response = _userService.LoginUser(model);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
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
