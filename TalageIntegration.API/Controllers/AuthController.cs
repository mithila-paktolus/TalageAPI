using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Talage.Authentication;
using TalageIntegraion.Auth0.Models;
using TalageIntegration.API.Auth0.Models;

namespace TalageIntegration.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login(AuthKeyRequest request)
        {
            var token = _jwtService.Authenticate(request);

            if (token != null)
            {
                return Ok(new AuthKeyResponse
                {
                    Status = "Success",
                    Token = token
                });
            }

            return Unauthorized(new { Message = "Invalid API Key or Secret" });
        }
    }
}
