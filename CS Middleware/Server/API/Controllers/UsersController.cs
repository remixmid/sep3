using API.CoreConnection;
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
        public async Task GetUser(int id) {
            Console.WriteLine($"Getting user with ID: {id}");
            await userClient.GetUserById(id);
        }

        [HttpPost]
        public async Task AddUser() {
            
        }
        

    }
}
