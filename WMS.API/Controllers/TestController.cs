using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok("Public endpoint");
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok("Protected endpoint");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok("Admin endpoint");
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new Exception("Test exception");
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me(
        [FromServices] ICurrentUserService currentUser)
    {
        return Ok(new
        {
            currentUser.UserId,
            currentUser.Username,
            currentUser.Role,
            currentUser.EmployeeId,
            currentUser.DepartmentId
        });
    }
}
