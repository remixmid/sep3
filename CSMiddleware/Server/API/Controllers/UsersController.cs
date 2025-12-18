using API.CoreConnection;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserClient _users;

    public UsersController(UserClient users) => _users = users;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] long id)
        => Ok(await _users.GetUserById(id));

    [HttpGet("by-username")]
    public async Task<ActionResult<UserDTO>> GetByUsername([FromQuery] string username)
        => Ok(await _users.GetUserByUsername(username));

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterUserRequest req)
    {
        var created = await _users.Register(req);
        return Created($"/api/users/{created.Id}", created);
    }
}