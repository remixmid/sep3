using System;

namespace DTOs.ChatDTOs;

public class AttachmentDTO {
    public required String Id { get; set; }
    public required String Type { get; set; }
    public required String Url { get; set; }
    public required long SizeBytes { get; set; }
    public required String MimeType { get; set; }

}
