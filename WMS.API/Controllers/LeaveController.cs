using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Leave;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeavesController(
        ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost("apply")]
    public async Task<IActionResult>
        ApplyLeave(
            ApplyLeaveRequestDto request)
    {
        await _leaveService
            .ApplyLeaveAsync(request);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Leave applied successfully"));
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult>
        CancelLeave(int id)
    {
        await _leaveService
            .CancelLeaveAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Leave cancelled successfully"));
    }

    [HttpGet("my-leaves")]
    public async Task<IActionResult>
        GetMyLeaves()
    {
        var response =
            await _leaveService
                .GetMyLeavesAsync();

        return Ok(
            ApiResponse<List<LeaveResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetLeaves(
            [FromQuery]
            LeaveFilterDto filter)
    {
        var response =
            await _leaveService
                .GetLeavesAsync(filter);

        return Ok(
            ApiResponse<List<LeaveResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetPendingLeaves()
    {
        var response =
            await _leaveService
                .GetPendingLeavesAsync();

        return Ok(
            ApiResponse<List<LeaveResponseDto>>
            .SuccessResponse(response));
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        ApproveLeave(int id)
    {
        await _leaveService
            .ApproveLeaveAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Leave approved successfully"));
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        RejectLeave(int id)
    {
        await _leaveService
            .RejectLeaveAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Leave rejected successfully"));
    }
}
