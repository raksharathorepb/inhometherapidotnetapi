// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;

// public class ApplicationDbContext : DbContext
// {
//     public DbSet<User> Users { get; set; }

//     public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//         : base(options)
//     {
//     }
// }

// public class User
// {
//     public int Id { get; set; }
//     public string Email { get; set; } = string.Empty;
//     public string Password { get; set; } = string.Empty;
//     public string Role { get; set; } = string.Empty; // "Admin", "Therapist", "Patient"
// }

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// using YourAppNamespace.Models; // <-- Add this if needed

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
