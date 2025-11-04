using System;
using DTOs.ModelDTOs;

namespace DTOs.MessageDTOs;

public class SendMessageDTO {
    public required UserDTO Sender;
    public required MessageDTO Content;
    public required UserDTO Recipient;

}
