using API.Hubs;
using DTOs.MessageDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase {
        private readonly IHubContext<MessagesHub, IClientHandler> hub;

        public MessagesController(IHubContext<MessagesHub, IClientHandler> MessagesHub) {
            hub = MessagesHub;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO request) {
            await hub.Clients.All.ReceiveMessage(request.Sender, request.Content, request.Recipient);

            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task GetMessage() {
                Console.WriteLine("Get Message Blah");
        }
    }
}
