using System.Security.Claims;
using BusinessLayer.Interface;
using CacheLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace AddressBook_App.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/addressbook")]
    public class AddressBook_AppController : ControllerBase
    {
        private readonly ILogger<AddressBook_AppController> _logger;
        private readonly IAddressBookService _addressBookService;
        private readonly IRedisCacheService _redisCacheService;

        public AddressBook_AppController(ILogger<AddressBook_AppController> logger, IAddressBookService addressBookService, IRedisCacheService redisCacheService)
        {
            _logger = logger;
            _addressBookService = addressBookService;
            _redisCacheService = redisCacheService;
        }
        private int GetUserIdFromToken()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Invalid or missing user ID in token.");
                throw new UnauthorizedAccessException(userId);
            }
            return int.Parse(userId);
        }
        /// <summary>
        /// Gets all address book entries.
        /// </summary>
        /// <returns>A list of all entries in the address book.</returns>
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation("Checking cache for contacts...");
                var cacheKey = $"contacts_user_{userId}";
                var cachedcontacts = await _redisCacheService.GetCachedData<List<AddressBookEntity>>(cacheKey);

                if (cachedcontacts != null)
                {
                    _logger.LogInformation("Returning greetings from cache.");
                    return Ok(new ResponseModel<List<AddressBookEntity>>
                    {
                        Success = true,
                        Message = $"Cache hit for user {userId} contacts",
                        Data = cachedcontacts
                    });
                }
                _logger.LogInformation("Fetching all contacts from Database...");
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
                await _redisCacheService.SetCachedData("all_contacts", contacts, TimeSpan.FromMinutes(20));
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
        [Authorize]
        public async Task<IActionResult> GetContactById(int id)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation($"Fetching Contact for UserID: {userId}, ContactID: {id}");
                var cacheKey = $"contact_{id}";
                var cachedContact = await _redisCacheService.GetCachedData<AddressBookEntity>(cacheKey);
                if (cachedContact != null)
                {
                    if (cachedContact.UserId != userId)
                    {
                        _logger.LogWarning($"Unauthorized access attempt by UserId {userId} for Contact ID {id}");
                        return Unauthorized(new ResponseModel<string> { Success = false, Message = "Unauthorized access" });
                    }
                    _logger.LogInformation($"Cache hit for contact ID: {id}");
                    return Ok(new ResponseModel<AddressBookEntity> { Success = true, Message = "Contact found (from cache)", Data = cachedContact });
                }
                _logger.LogInformation($"Fetching greeting from DB for UserID: {userId}, GreetingID: {id}");
                var contact=_addressBookService.GetContactById(userId, id);
                if (contact == null)
                    return NotFound(new ResponseModel<AddressBookEntity> { Success = false, Message = "Contact not found" });
                await _redisCacheService.SetCachedData(cacheKey, contact, TimeSpan.FromMinutes(20));
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
        [Authorize]
        public IActionResult AddContact(string contactName, string contactNumber,string email, string address)
        {
            _logger.LogInformation("Trying to Save the Contact");
            try
            {
                int userId = GetUserIdFromToken();
                var user=_addressBookService.AddContact(userId, contactName, contactNumber,email,address);
                if(user==null)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Contact not Saved!",
                        Data = $"Name: {user.ContactName}, Number: {user.ContactNumber}, Email: {user.Email} and Address: {user.Address}"
                    });
                }
                _logger.LogInformation("Saving the Contact...");
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Contact saved successfully",
                    Data = $"Name: {user.ContactName}, Number: {user.ContactNumber}, Email: {user.Email} and Address: {user.Address}"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Unauthorized access: {ex.Message}");
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Unauthorized access." });
            }
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Error in AddContact: {ex.Message}");
            //    return StatusCode(500, new ResponseModel<string> { Success = false, Message = "Internal Server Error" });
            //}
        }
        /// <summary>
        /// Updates an existing address book contact.
        /// </summary>
        /// <returns>The updated contact.</returns>
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateContactById(int id, string name, string number,string email, string address)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation($"Attempting to update contact with ID: {id}");

                bool isUpdated = _addressBookService.UpdateContact(userId, id, name, number, email,address);

                if (!isUpdated)
                {
                    _logger.LogWarning($"Contact with ID {id} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found."
                    });
                }
                // Removing Cache since data changed
                await _redisCacheService.RemoveCachedData("all_contacts");
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = $"Contact with ID {id} updated successfully.",
                    Data = $"New Contact Name: {name}    New Contact Number: {number}   New Email: {email}   New Address: {address}"
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
        [Authorize]
        public async Task<IActionResult> DeleteContactById( int id)
        {
            try
            {
                int userId = GetUserIdFromToken();
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
                // Removing Cache
                await _redisCacheService.RemoveCachedData("all_contacts");
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
        /// <summary>
        /// View List of All Contacts For Admin
        /// </summary>
        /// <returns>List of All User Contacts</returns>
       
        [HttpGet("admin/contacts")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllContactsForAdmin()
        {
            try
            {
                _logger.LogInformation("fetching All Contacts for Admin...");
                var contacts = _addressBookService.GetAllContactsForAdmin();
                var response = new ResponseModel<List<AddressBookEntity>>();
                if (contacts == null || !contacts.Any())
                {
                    response.Success = false;
                    response.Message = "No contacts found.";
                    return NotFound(response);
                }
                response.Success = true;
                response.Message = "Contacts found.";
                response.Data = contacts;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
        /// <summary>
        /// Delete any Contact For Admin
        /// </summary>
        /// <returns>Contact Deleted</returns>
        
        [HttpDelete("admin/contacts/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContactByAdmin(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting Contacts {id} for Admin...");
                bool isDeleted = _addressBookService.DeleteContactByAdmin(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = "Contact not found." });
                }
                await _redisCacheService.RemoveCachedData("all_contacts");
                return Ok(new { message = "Contact deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
