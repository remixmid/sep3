using API.CoreConnection;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {

        private UserClient userClient;

        public UsersController(UserClient client) {
            userClient = client;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id) {
            Console.WriteLine($"Getting user with ID: {id}");
            return await userClient.GetUserById(id);
        }

        [HttpGet("search")]
        public async Task<ActionResult<UserDTO>> GetUserByName([FromQuery] String username) {
            Console.WriteLine($"Getting user with name: {username}");
            return await userClient.GetUserByUsername(username);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> AddUser(RegisterUserRequest req) {
            UserDTO res = await userClient.Register(req);
            return Created($"/user/{res.Id}", res);
        }
    }
}
