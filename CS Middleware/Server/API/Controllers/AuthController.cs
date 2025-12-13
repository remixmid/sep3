using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.CoreConnection;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IConfiguration config;
        private readonly UserClient httpClient;

        
        public AuthController(IConfiguration configuration, UserClient client) {
            config = configuration;
            httpClient = client;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req) {
            var user = await Authenticate(req.Username, req.Password);

            if(user == null)
                return Unauthorized();

            var token = GenerateToken(user);

            return Ok(new {token});
        }

        private String GenerateToken(UserDTO user) {
            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, user.Username),
                new("user_id", user.Id.ToString())
            ];

            // This exists in appsettings.json, it's not null.
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwt = new(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                // A better approach for this should probably be used to avoid reusing tokens. Maybe pinging a connection every 15 minutes to see if still active and then close if not.
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async ValueTask<UserDTO> Authenticate(String username, String password) {
            // I need some sort of endpoint on the Core Java server for getting both username + password.
            // var res = await httpClient.GetUserByUsername(username, password);
            throw new NotImplementedException();
        }
    }
}
