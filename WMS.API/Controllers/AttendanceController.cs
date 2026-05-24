using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Interfaces;
using WMS.Application.Common.Models;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(AttendanceRequestDto request)
    {
        var response = await _attendanceService.CheckInAsync(request);
        return Ok(ApiResponse<AttendanceResponseDto>.SuccessResponse(response, "Check-in successful"));
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut()
    {
        var response = await _attendanceService.CheckOutAsync();
        return Ok(ApiResponse<AttendanceResponseDto>.SuccessResponse(response, "Check-out successful"));
    }

    [HttpGet("today")]
    public async Task<IActionResult>
        GetTodayAttendance()
    {
        var response =
            await _attendanceService
                .GetTodayAttendanceAsync();

        return Ok(
            ApiResponse<TodayAttendanceDto>
            .SuccessResponse(response));
    }

    [HttpGet("my-monthly-report")]
    public async Task<IActionResult>
        GetMyMonthlyReport()
    {
        var response =
            await _attendanceService
                .GetMonthlyReportAsync();

        return Ok(
            ApiResponse<MonthlyAttendanceReportDto>
            .SuccessResponse(response));
    }

    [HttpGet("my-history")]
    public async Task<IActionResult>
        GetMyHistory(
            [FromQuery] AttendanceFilterDto filter)
    {
        filter.EmployeeId = null;

        var response =
            await _attendanceService
                .GetMyAttendanceAsync(filter);

        return Ok(
            ApiResponse<PagedResponse<AttendanceResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetAttendanceHistory(
            [FromQuery] AttendanceFilterDto filter)
    {
        var response =
            await _attendanceService
                .GetAttendanceHistoryAsync(filter);

        return Ok(
            ApiResponse<PagedResponse<AttendanceResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("employee/{employeeId}/monthly-report")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetEmployeeMonthlyReport(
            int employeeId)
    {
        var response =
            await _attendanceService
                .GetMonthlyReportAsync(employeeId);

        return Ok(
            ApiResponse<MonthlyAttendanceReportDto>
            .SuccessResponse(response));
    }

    [HttpGet("absentees")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetAbsentees()
    {
        var response =
            await _attendanceService
                .GetAbsenteesAsync();

        return Ok(
            ApiResponse<List<AbsenteeDto>>
            .SuccessResponse(response));
    }
}
