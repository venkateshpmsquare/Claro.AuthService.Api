using Claro.AuthService.Domain.Entities;
using Claro.AuthService.Infrastructure.Interfaces;
using Claro.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claro.AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ClaroAuthDbContext _dbContext;

        public UserRepository(ClaroAuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
