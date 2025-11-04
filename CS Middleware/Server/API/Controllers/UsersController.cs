using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {

        [HttpGet]
        public async Task GetUser() {

        }

        [HttpPost]
        public async Task AddUser() {
            
        }
        

    }
}
