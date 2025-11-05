using System.ComponentModel.DataAnnotations;

namespace Model;

public class User {
    
    /* 
     * We should probably use UUIDs 
     */
    public int ID { get; set; }
    public required String Username { get; set; }
    public required String DisplayName { get; set; }
    public required String Password { get; set; }
}
