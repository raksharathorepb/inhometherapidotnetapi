
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

 [HttpPost("create-therapist")]
public async Task<IActionResult> CreateTherapist([FromBody] CreateTherapistDto model)
{
    try
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
            return BadRequest("Therapist already exists.");

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name,
            Branch = model.Branch,
            Role = "Therapist"
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(errors); // returns errors like "Password too weak"
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Therapist");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            return BadRequest(errors); // e.g., role doesn't exist
        }

        return Ok("Therapist created successfully.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

}

public class CreateTherapistDto
{
    public required string Name { get; set; }
    public required string Branch { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
