using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok("Patient dashboard data");
    }
}
