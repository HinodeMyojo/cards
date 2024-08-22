﻿using CardsServer.BLL.Abstractions;
using CardsServer.BLL.Entity;
using Microsoft.EntityFrameworkCore;

namespace CardsServer.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> GetUser(int userId, CancellationToken cancellationToken)
        {
            UserEntity? user = await _context.Users.FindAsync(userId, cancellationToken);

            return user;
        }

        public async Task<UserEntity?> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            UserEntity? user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }
}
    }
