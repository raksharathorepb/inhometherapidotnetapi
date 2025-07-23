using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    internal string password;

    public required string? Role { get; set; }
    public string? Name { get; set; }     // Optional, use required if needed
    public string? Branch { get; set; }

    public string? Email { get; set; }   
}

