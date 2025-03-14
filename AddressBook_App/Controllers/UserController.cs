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

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            var response = _userService.RegisterUser(model);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            var response = _userService.LoginUser(model);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgetPasswordRequest model)
        {
            var response = _userService.ForgotPassword(model);
            return Ok(response);
        }

        [HttpPost]
        [Route("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var response = _userService.ResetPassword(model);
            return Ok(response);
        }
    }
}
