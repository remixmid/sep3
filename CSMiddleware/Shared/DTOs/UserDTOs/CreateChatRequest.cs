using System.Text.Json.Serialization;

namespace DTOs.UserDTOs;

public class CreateChatRequest
{
    public ChatType Type { get; set; }

    public string? Title { get; set; }

    public long OwnerId { get; set; }

    public List<long> MemberIds { get; set; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatType
{
    DIRECT,
    GROUP,
    CHANNEL
}