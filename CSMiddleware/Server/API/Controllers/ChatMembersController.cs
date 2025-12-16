using API.CoreConnection;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/chats/{chatId:long}/members")]
public class ChatMembersController : ControllerBase
{
    private readonly ChatMemberClient _members;

    public ChatMembersController(ChatMemberClient members) => _members = members;

    [HttpGet]
    public async Task<ActionResult<List<UserInChatDTO>>> GetMembers([FromRoute] long chatId)
        => Ok(await _members.GetChatMembers(chatId));

    [HttpPost]
    public async Task<ActionResult<UserInChatDTO>> AddMember([FromRoute] long chatId, [FromQuery] long userId)
        => Ok(await _members.AddMemberToChat(chatId, userId));

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> RemoveMember([FromRoute] long chatId, [FromRoute] long userId)
    {
        await _members.RemoveMemberFromChat(chatId, userId);
        return NoContent();
    }

    [HttpPost("{userId:long}/block")]
    public async Task<IActionResult> Block([FromRoute] long chatId, [FromRoute] long userId)
    {
        await _members.BlockMember(chatId, userId);
        return NoContent();
    }

    [HttpPost("{userId:long}/unblock")]
    public async Task<IActionResult> Unblock([FromRoute] long chatId, [FromRoute] long userId)
    {
        await _members.UnblockMember(chatId, userId);
        return NoContent();
    }
}