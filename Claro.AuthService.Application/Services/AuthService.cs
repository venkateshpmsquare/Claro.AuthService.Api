using Claro.AuthService.Application.Contracts.Authentication;
using Claro.AuthService.Application.Interfaces;
using Claro.AuthService.Domain.Entities;
using Claro.AuthService.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claro.AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseModel> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            LoginResponseModel loginResponse = new LoginResponseModel();
            loginResponse.Token = _jwtTokenGenerator.GenerateToken(user);
            loginResponse.Username = user.Username;
            loginResponse.Email = user.Email;
            loginResponse.Role = user.Role;
            return loginResponse;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task AddAsync(User user)
        {
            await _userRepository.AddAsync(user);
        }
    }
}
