// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// [Authorize(Roles = "Therapist")]
// [ApiController]
// [Route("api/[controller]")]
// public class TherapistController : ControllerBase
// {
//     [HttpGet("dashboard")]
//     public IActionResult GetDashboard()
//     {
//         return Ok("Therapist dashboard data");
//     }
// }
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// using YourAppNamespace.Data; // Replace with your actual namespace
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class TherapistController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TherapistController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Therapist's own dashboard
    [Authorize(Roles = "Therapist")]
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok("Therapist dashboard data");
    }

    // Admin can view all therapists
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAllTherapists()
    {
        var therapists = _context.Users
            .Where(u => u.Role == "Therapist")
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.Branch
            })
            .ToList();

        return Ok(therapists);
    }
}
