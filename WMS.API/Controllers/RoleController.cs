using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Role;
using WMS.Infrastructure.Data;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly WmsDbContext _context;

    public RolesController(
        WmsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetRoles()
    {
        var roles =
            await _context.Roles
                .Select(role =>
                    new RoleResponseDto
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName
                    })
                .ToListAsync();

        return Ok(
            ApiResponse<List<RoleResponseDto>>
                .SuccessResponse(roles)
        );
    }
}
