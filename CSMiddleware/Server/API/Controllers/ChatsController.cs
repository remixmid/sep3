using System.Security.Claims;
using API.CoreConnection;
using DTOs.ChatDTOs;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers;

[ApiController]
[Route("api/chats")]
public class ChatsController : ControllerBase
{
    private readonly ChatClient _chats;

    public ChatsController(ChatClient chats) => _chats = chats;

    private static string MaskAuth(string? auth)
    {
        if (string.IsNullOrWhiteSpace(auth)) return "<empty>";
        var s = auth.Trim();
        if (s.Length <= 18) return s;
        return s[..12] + "..." + s[^6..];
    }

    private void LogIncoming(string action)
    {
        var trace = HttpContext?.TraceIdentifier ?? "<no-trace>";
        var auth = Request.Headers["Authorization"].ToString();
        var isAuth = User?.Identity?.IsAuthenticated ?? false;
        var sub = User?.FindFirst("sub")?.Value
                  ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? "<no-sub>";

        Console.WriteLine(
            $"[GW][{trace}] {action} IN  {Request.Method} {Request.Path}{Request.QueryString} | " +
            $"IsAuthenticated={isAuth} sub={sub} Auth={MaskAuth(auth)}");
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatDTO>>> GetUserChats([FromQuery] long userId)
    {
        LogIncoming($"GetUserChats(userId={userId})");
        return Ok(await _chats.GetChatsForUser(userId));
    }

    [HttpGet("{chatId:long}")]
    public async Task<ActionResult<ChatDTO>> GetChatById([FromRoute] long chatId)
    {
        LogIncoming($"GetChatById(chatId={chatId})");
        return Ok(await _chats.GetChatById(chatId));
    }

    [HttpGet("{chatId:long}/messages")]
    public async Task<ActionResult<List<MessageDTO>>> GetChatMessages(
        [FromRoute] long chatId,
        [FromQuery] int page = 0,
        [FromQuery] int size = 50)
    {
        LogIncoming($"GetChatMessages(chatId={chatId}, page={page}, size={size})");
        return Ok(await _chats.GetMessagesInChat(chatId, page, size));
    }

    [HttpPost]
    public async Task<ActionResult<ChatDTO>> CreateChat([FromBody] CreateChatRequest req)
    {
        LogIncoming("CreateChat");

        var trace = HttpContext?.TraceIdentifier ?? "<no-trace>";
        try
        {
            var members = req.MemberIds is null ? "<null>" : string.Join(",", req.MemberIds);
            Console.WriteLine(
                $"[GW][{trace}] CreateChat BODY type={req.Type} title='{req.Title}' ownerId={req.OwnerId} memberIds=[{members}]");

            return Ok(await _chats.CreateNewChat(req));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GW][{trace}] CreateChat EXCEPTION: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    [HttpDelete("{chatId:long}/for-user/{userId:long}")]
    public async Task<IActionResult> DeleteChatForUser([FromRoute] long chatId, [FromRoute] long userId)
    {
        LogIncoming($"DeleteChatForUser(chatId={chatId}, userId={userId})");
        await _chats.DeleteChatForUser(chatId, userId);
        return NoContent();
    }

    [HttpDelete("{chatId:long}")]
    public async Task<IActionResult> DeleteChatForAll([FromRoute] long chatId)
    {
        LogIncoming($"DeleteChatForAll(chatId={chatId})");
        await _chats.DeleteChatForAll(chatId);
        return NoContent();
    }
}
