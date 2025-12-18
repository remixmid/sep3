using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Hubs;
using DTOs.ChatDTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Services;

public class RealtimePublisher
{
    private readonly IHubContext<UserHub> _hub;
    private readonly ILogger<RealtimePublisher> _log;

    public RealtimePublisher(IHubContext<UserHub> hub, ILogger<RealtimePublisher> log)
    {
        _hub = hub;
        _log = log;
    }

    public async Task MessageCreated(long chatId, MessageDTO message, IReadOnlyCollection<long> memberIds, long senderId)
    {
        var payload = new
        {
            chatId,
            senderId,
            message,
            ts = DateTimeOffset.UtcNow
        };

        await SafeSendGroup(chatId.ToString(), "MessageCreated", payload);
        await SafeSendGroup($"chat:{chatId}", "MessageCreated", payload);

        var userGroups = memberIds
            .Where(id => id != senderId)
            .Select(id => $"user:{id}")
            .Distinct()
            .ToList();

        if (userGroups.Count > 0)
        {
            await SafeSendGroups(userGroups, "MessageNotification", payload);
        }

        _log.LogInformation("[RT] MessageCreated chatId={ChatId} senderId={SenderId} members={MembersCount}",
            chatId, senderId, memberIds?.Count ?? 0);
    }

    public async Task MessageEdited(long chatId, string messageId, long editorId, object patchOrMessageDto)
    {
        var payload = new
        {
            chatId,
            editorId,
            messageId,
            data = patchOrMessageDto,
            ts = DateTimeOffset.UtcNow
        };

        await SafeSendGroup(chatId.ToString(), "MessageEdited", payload);
        await SafeSendGroup($"chat:{chatId}", "MessageEdited", payload);
    }

    public async Task MessageDeleted(long chatId, string messageId, long userId, bool forAll)
    {
        var payload = new
        {
            chatId,
            userId,
            messageId,
            forAll,
            ts = DateTimeOffset.UtcNow
        };

        await SafeSendGroup(chatId.ToString(), "MessageDeleted", payload);
        await SafeSendGroup($"chat:{chatId}", "MessageDeleted", payload);
    }

    private async Task SafeSendGroup(string group, string method, object payload)
    {
        try
        {
            await _hub.Clients.Group(group).SendAsync(method, payload);
        }
        catch (Exception e)
        {
            _log.LogWarning(e, "[RT] Failed send to Group={Group} Method={Method}", group, method);
        }
    }

    private async Task SafeSendGroups(IReadOnlyList<string> groups, string method, object payload)
    {
        try
        {
            await _hub.Clients.Groups(groups).SendAsync(method, payload);
        }
        catch (Exception e)
        {
            _log.LogWarning(e, "[RT] Failed send to Groups={Groups} Method={Method}", string.Join(",", groups), method);
        }
    }
}
