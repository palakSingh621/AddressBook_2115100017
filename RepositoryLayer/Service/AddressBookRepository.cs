using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRepository: IAddressBookRepository
    {
        private readonly AddressBookContext _context;
        public AddressBookRepository(AddressBookContext context)
        {
            _context = context;
        }
        public List<AddressBookEntity> GetAllContact(int userId)
        {
            return _context.AddressBookContacts.Where(g=> g.UserId==userId).ToList();
        }
        public AddressBookEntity GetContactById(int userId,int id)
        {
            return _context.AddressBookContacts.FirstOrDefault(g=> g.UserId== userId && g.Id == id);
        }
        public AddressBookEntity AddContact(int userId, string name, string number, string email, string address)
        {
            var user = _context.Users.FirstOrDefault(g => g.Id== userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            var contact= new AddressBookEntity
            {
                ContactName = name,
                ContactNumber = number,
                Email = email,
                Address = address,
                UserId = userId
            };
            _context.AddressBookContacts.Add(contact);
            _context.SaveChanges();
            return contact;
        }
        public bool UpdateContact(int userId, int id, string newName, string newNumber, string email, string address)
        {
            var contact=_context.AddressBookContacts.FirstOrDefault(g => g.Id == id && g.UserId == userId);
            if(contact == null)
            {
                return false;
            }
            contact.ContactName = newName;
            contact.ContactNumber = newNumber;
            contact.Email = email;
            contact.Address = address;
            _context.SaveChanges();
            return true;
        }
        public bool DeleteContact(int userId, int id)
        {
            var contact = _context.AddressBookContacts.FirstOrDefault(g => g.Id == id && g.UserId == userId);
            if (contact == null)
            {
                return false;
            }
            _context.AddressBookContacts.Remove(contact);
            _context.SaveChanges();
            return true;
        }
        public List<AddressBookEntity> GetAllContactsForAdmin()
        {
            return _context.AddressBookContacts.ToList();
        }
        public bool DeleteContactByAdmin(int contactId)
        {
            var contact = _context.AddressBookContacts.FirstOrDefault(g => g.Id == contactId);
            if (contact == null)
                return false;

            _context.AddressBookContacts.Remove(contact);
            _context.SaveChanges();
            return true;
        }
    }
}
