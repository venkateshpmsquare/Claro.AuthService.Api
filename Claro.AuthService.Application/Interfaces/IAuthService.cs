using Claro.AuthService.Application.Contracts.Authentication;
using Claro.AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claro.AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseModel> AuthenticateAsync(string username, string password);
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
