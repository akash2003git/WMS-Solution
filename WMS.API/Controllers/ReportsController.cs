using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.Interfaces;
using WMS.Domain.Enums;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("attendance/monthly/{employeeId}")]
    public async Task<IActionResult> MonthlyAttendance(
        int employeeId,
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var response =
            await _reportService
                .GetMonthlyAttendanceReportAsync(
                    employeeId,
                    year,
                    month);

        return Ok(ApiResponse<object>
            .SuccessResponse(response));
    }

    [HttpGet("attendance/history/{employeeId}")]
    public async Task<IActionResult> AttendanceHistory(
        int employeeId)
    {
        var response =
            await _reportService
                .GetAttendanceHistoryAsync(employeeId);

        return Ok(ApiResponse<object>
            .SuccessResponse(response));
    }

    [HttpGet("leaves")]
    public async Task<IActionResult> LeaveReport(
        [FromQuery] int? employeeId,
        [FromQuery] LeaveStatus? status)
    {
        var response =
            await _reportService
                .GetLeaveReportAsync(employeeId, status);

        return Ok(ApiResponse<object>
            .SuccessResponse(response));
    }

    [HttpGet("employees/department/{departmentId}")]
    public async Task<IActionResult> DepartmentEmployees(
        int departmentId)
    {
        var response =
            await _reportService
                .GetDepartmentEmployeeReportAsync(departmentId);

        return Ok(ApiResponse<object>
            .SuccessResponse(response));
    }

    [HttpGet("projects/allocations")]
    public async Task<IActionResult> ProjectAllocations()
    {
        var response =
            await _reportService
                .GetProjectAllocationReportAsync();

        return Ok(ApiResponse<object>
            .SuccessResponse(response));
    }
}
