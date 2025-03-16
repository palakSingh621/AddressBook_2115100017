﻿using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        bool UserExists(string email);
        UserEntity CreateUser(string username, string email, string passwordHash, string role);
        UserEntity GetUserByEmail(string email);
        bool UpdateUserPassword(string email, string newPasswordHash);
        UserEntity GetUserById(int userId);
    }
}
