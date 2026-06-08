using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UsermanagementApi.Models;
using UsermanagementApi.Services;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace UsermanagementApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        private readonly IConfiguration _config;

        public UsersController(UserService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        //  Protected GET
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetUsers());
        }

        //  Protected POST (Create user)
        [Authorize]
        [HttpPost]
        public IActionResult Post(User user)
        {
            return Ok(_service.AddUser(user));
        }

        //  LOGIN (no authorize)
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginDto login)
        {
            var user = _service.GetUsers()
                .FirstOrDefault(u => u.Name == login.Name && u.Password == login.Password);

            if (user == null)
                return Unauthorized("Invalid Credentials");

            var token = GenerateToken(user);

            return Ok(new { token });
        }

        //  TOKEN GENERATION
        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}