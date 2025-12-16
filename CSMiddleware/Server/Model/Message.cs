using System;
using System.ComponentModel.DataAnnotations;

namespace Model;

public class Message {

    /*
     * These fields are mostly just copied over from the Java model.
     */
    public long AuthorID;
    public long ID;
    public long RecieverID;
    public int ConversationID;
    public required String Content;
}
