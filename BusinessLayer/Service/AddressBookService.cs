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
        public AddressBookEntity AddContact(int userId, string name, string number)
        {
            return _addressBookRepository.AddContact(userId, name, number);
        }
        public bool UpdateContact(int userId, int id, string newName, string newNumber)
        {
            return _addressBookRepository.UpdateContact(userId, id, newName, newNumber);
        }
        public bool DeleteContact(int userId, int id)
        {
            return _addressBookRepository.DeleteContact(userId, id);
        }
    }
}
