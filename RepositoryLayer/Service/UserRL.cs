﻿using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookContext _context;

        public UserRL(AddressBookContext context)
        {
            _context = context;
        }

        public bool UserExists(string email) => _context.Users.Any(u => u.Email == email);

        public void CreateUser(string username, string email, string passwordHash)
        {
            var user = new UserEntity { UserName = username, Email = email, PasswordHash = passwordHash };
            _context.Users.Add(user);
        }
        public UserEntity GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void UpdateUserPassword(string email, string newPasswordHash)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.PasswordHash = newPasswordHash;
            }
        }
    }
}
