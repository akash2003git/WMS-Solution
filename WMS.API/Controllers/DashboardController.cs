using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Dashboard;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController
    : ControllerBase
{
    private readonly IDashboardService
        _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService =
            dashboardService;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        GetAdminDashboard()
    {
        var response =
            await _dashboardService
                .GetAdminDashboardAsync();

        return Ok(
            ApiResponse<AdminDashboardDto>
            .SuccessResponse(response));
    }

    [HttpGet("manager")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetManagerDashboard()
    {
        var response =
            await _dashboardService
                .GetManagerDashboardAsync();

        return Ok(
            ApiResponse<ManagerDashboardDto>
            .SuccessResponse(response));
    }

    [HttpGet("employee")]
    public async Task<IActionResult>
        GetEmployeeDashboard()
    {
        var response =
            await _dashboardService
                .GetEmployeeDashboardAsync();

        return Ok(
            ApiResponse<EmployeeDashboardDto>
            .SuccessResponse(response));
    }
}
