using Claro.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Claro.AuthService.Application.Contracts.Authentication;
using Claro.AuthService.Domain.Entities;
using Claro.AuthService.Application.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
namespace Claro.AuthService.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly PasswordHasher _passwordHasher;
        private readonly TelemetryClient _telemetryClient;
        public AuthController(IAuthService authService, PasswordHasher passwordHasher, TelemetryClient telemetryClient)
        {
            _authService = authService;
            _passwordHasher = passwordHasher;
            telemetryClient = telemetryClient;
        }

        [HttpGet("test-telemetry")]
        public IActionResult TestTelemetry()
        {
            // Track a Custom Event
            _telemetryClient.TrackEvent("TestTelemetryEvent");

            // Track a Custom Trace
            _telemetryClient.TrackTrace("This is a test trace from Claro.AuthService.Api", SeverityLevel.Information);

            // Track a Custom Exception
            try
            {
                throw new Exception("This is a test exception for Application Insights.");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
            }

            return Ok("Telemetry sent successfully!");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Application.Contracts.Authentication.RegisterRequest request)
        {
            // Check if the user already exists
            var existingUser = await _authService.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            // Hash the password
            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            // Create new user entity
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = request.Role
            };
            // Store user in database
            await _authService.AddAsync(user);

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Application.Contracts.Authentication.LoginRequest request)
        {
            var token = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (token == null)
                return Unauthorized();

            return Ok(token);
        }
    }
}
