using Microsoft.AspNetCore.Mvc;

namespace AddressBook_App.Controllers
{
    [ApiController]
    [Route("[api/addressbook]")]
    public class AddressBook_AppController : ControllerBase
    {
        private readonly ILogger<AddressBook_AppController> _logger;

        public AddressBook_AppController(ILogger<AddressBook_AppController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public string Get()
        {
            return "This is AddressBookProject";
        }
    }
}
