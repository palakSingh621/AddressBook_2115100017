using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace AddressBook_App.Controllers
{
    [ApiController]
    [Route("api/addressbook")]
    public class AddressBook_AppController : ControllerBase
    {
        private readonly ILogger<AddressBook_AppController> _logger;
        private readonly IAddressBookService _addressBookService;

        public AddressBook_AppController(ILogger<AddressBook_AppController> logger, IAddressBookService addressBookService)
        {
            _logger = logger;
            _addressBookService = addressBookService;
        }
        /// <summary>
        /// Gets all address book entries.
        /// </summary>
        /// <returns>A list of all entries in the address book.</returns>
        [HttpGet]
        public IActionResult GetAllContacts(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching All Contacts...");
                var contacts = _addressBookService.GetAllContact(userId);
                if (contacts == null || contacts.Count == 0)
                {
                    _logger.LogWarning("No Contacts found.");
                    return NotFound(new ResponseModel<List<AddressBookEntity>>
                    {
                        Success = false,
                        Message = "No contacts found."
                    });
                }
                var response = new ResponseModel<List<AddressBookEntity>>
                {
                    Success = true,
                    Message = "Contacts Fetched successfully!",
                    Data = contacts
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllContacts: {ex.Message}");
                return StatusCode(500, new ResponseModel<List<AddressBookEntity>>
                {
                    Success = false,
                    Message = "Internal Server Error"
                });
            }
        }
        /// <summary>
        /// Gets a specific address book contact by ID.
        /// </summary>
        /// <returns>The address book contact with the given ID.</returns>
        [HttpGet("{id}")]
        public IActionResult GetContactById(int userId, int id)
        {
            try
            {
                _logger.LogInformation($"Fetching Contact for UserID: {userId}, ContactID: {id}");
                var contact=_addressBookService.GetContactById(userId, id);
                if (contact == null)
                    return NotFound(new ResponseModel<AddressBookEntity> { Success = false, Message = "Contact not found" });
                return Ok(new ResponseModel<AddressBookEntity> { Success = true, Message = "Contact found", Data = contact });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetContactById method");
                return StatusCode(500, "Internal Server Error");
            }
        }
        /// <summary>
        /// Adds a new address book contact.
        /// </summary>
        /// <returns>The newly added contact.</returns>
        [HttpPost]
        public IActionResult AddContact([FromBody] int userId,string contactName, string contactNumber)
        {
            try
            { 
                _addressBookService.AddContact(userId, contactName, contactNumber);
                _logger.LogInformation("Saving the Contact...");

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Contact saved successfully",
                    Data = "Contact Saved!"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Unauthorized access: {ex.Message}");
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Unauthorized access." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddContact: {ex.Message}");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "Internal Server Error" });
            }
        }
        /// <summary>
        /// Updates an existing address book contact.
        /// </summary>
        /// <returns>The updated contact.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContactById(int userId, int id, string name, string number)
        {
            try
            {
                _logger.LogInformation($"Attempting to update contact with ID: {id}");

                bool isUpdated = _addressBookService.UpdateContact(userId, id, name, number);

                if (!isUpdated)
                {
                    _logger.LogWarning($"Contact with ID {id} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found."
                    });
                }
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = $"Contact with ID {id} updated successfully.",
                    Data = $"New Contact Name: {name} and New Contact Number: {number}"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateContactById: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal Server Error"
                });
            }
        }
        /// <summary>
        /// Deletes an address book contact.
        /// </summary>
        /// <returns>A success response after deletion.</returns>
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteContactById(int userId, int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete contact with ID: {id}");

                bool isDeleted = _addressBookService.DeleteContact(userId, id);

                if (!isDeleted)
                {
                    _logger.LogWarning($"Contact with ID {id} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Contact not found."
                    });
                }
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Contact deleted successfully.",
                    Data = $"Deleted Contact ID: {id}"
                };
                _logger.LogInformation($"Contact with ID {id} deleted successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteContactById: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
