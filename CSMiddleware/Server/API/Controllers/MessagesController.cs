using API.CoreConnection;
using API.Services;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly MessageClient _messages;
    private readonly ChatMemberClient _chatMembers;
    private readonly RealtimePublisher _realtime;

    public MessagesController(MessageClient messages, ChatMemberClient chatMembers, RealtimePublisher realtime)
    {
        _messages = messages;
        _chatMembers = chatMembers;
        _realtime = realtime;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> Send([FromBody] SendMessageRequest req)
    {
        var msg = await _messages.SendMessage(req);

        var senderId = TryGetUserId() ?? req.SenderId;

        var members = await _chatMembers.GetChatMembers(req.ChatId);
        var memberIds = members.Select(m => m.UserId).ToList();

        await _realtime.MessageCreated(req.ChatId, msg, memberIds, senderId);

        return Ok(msg);
    }

    [HttpPatch("{messageId}")]
    public async Task<ActionResult<MessageDTO>> Edit([FromRoute] string messageId, [FromBody] EditMessageRequest req)
    {
        var msg = await _messages.EditMessage(messageId, req);

        if (msg != null && msg.ChatId > 0)
        {
            var editorId = TryGetUserId() ?? 0;
            await _realtime.MessageEdited(msg.ChatId, messageId, editorId, msg);
        }

        return Ok(msg);
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteOne(
        [FromRoute] string messageId,
        [FromQuery] long userId,
        [FromQuery] bool forAll = false,
        [FromQuery] long? chatId = null
    )
    {
        await _messages.DeleteMessage(messageId, userId, forAll);

        if (chatId.HasValue && chatId.Value > 0)
        {
            await _realtime.MessageDeleted(chatId.Value, messageId, userId, forAll);
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMany([FromBody] DeleteMessagesRequest req, [FromQuery] long? chatId = null)
    {
        await _messages.DeleteManyMessages(req);

        if (chatId.HasValue && chatId.Value > 0 && req?.MessageIds != null)
        {
            foreach (var id in req.MessageIds)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    await _realtime.MessageDeleted(chatId.Value, id, req.UserId, req.ForAll);
                }
            }
        }

        return NoContent();
    }

    private long? TryGetUserId()
    {
        var sub = User.FindFirst("sub")?.Value
               ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(sub, out var id) ? id : null;
    }
}
