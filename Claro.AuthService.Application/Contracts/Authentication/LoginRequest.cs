using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claro.AuthService.Application.Contracts.Authentication
{
    public class LoginRequest
    {
        public string Username { get; set; }  // Ensure this property exists
        public string Password { get; set; }
    }
}
