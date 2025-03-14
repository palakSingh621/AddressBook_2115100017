﻿using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        bool UserExists(string email);
        UserEntity CreateUser(string username, string email, string passwordHash);
        UserEntity GetUserByEmail(string email);
        bool UpdateUserPassword(string email, string newPasswordHash);
    }
}
