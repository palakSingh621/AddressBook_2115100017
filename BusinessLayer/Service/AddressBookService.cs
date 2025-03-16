using BusinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookService: IAddressBookService
    {
        private readonly IAddressBookRepository _addressBookRepository;
        public AddressBookService(IAddressBookRepository addressBookRepository)
        {
            _addressBookRepository = addressBookRepository;
        }
        public List<AddressBookEntity> GetAllContact(int userId)
        {
            return _addressBookRepository.GetAllContact(userId);
        }
        public AddressBookEntity GetContactById(int userId, int id)
        {
            return _addressBookRepository.GetContactById(userId, id);   
        }
        public AddressBookEntity AddContact(int userId, string name, string number, string email, string address)
        {
            return _addressBookRepository.AddContact(userId, name, number, email, address);
        }
        public bool UpdateContact(int userId, int id, string newName, string newNumber,string email, string address)
        {
            return _addressBookRepository.UpdateContact(userId, id, newName, newNumber, email, address);
        }
        public bool DeleteContact(int userId, int id)
        {
            return _addressBookRepository.DeleteContact(userId, id);
        }
        public List<AddressBookEntity> GetAllContactsForAdmin()
        {
            return _addressBookRepository.GetAllContactsForAdmin();
        }
        public bool DeleteContactByAdmin(int contactId)
        {
            return _addressBookRepository.DeleteContactByAdmin(contactId);
        }
    }
}
