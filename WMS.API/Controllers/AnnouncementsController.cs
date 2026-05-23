using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementsController
    : ControllerBase
{
    private readonly IAnnouncementService
        _announcementService;

    public AnnouncementsController(
        IAnnouncementService announcementService)
    {
        _announcementService =
            announcementService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        CreateAnnouncement(
            CreateAnnouncementRequestDto request)
    {
        var response =
            await _announcementService
                .CreateAnnouncementAsync(
                    request);

        return Ok(
            ApiResponse<AnnouncementResponseDto>
            .SuccessResponse(
                response,
                "Announcement created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        UpdateAnnouncement(
            int id,
            UpdateAnnouncementRequestDto request)
    {
        await _announcementService
            .UpdateAnnouncementAsync(
                id,
                request);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Announcement updated successfully"));
    }

    [HttpPut("{id}/activate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        ActivateAnnouncement(int id)
    {
        await _announcementService
            .ActivateAnnouncementAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Announcement activated successfully"));
    }

    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        DeactivateAnnouncement(int id)
    {
        await _announcementService
            .DeactivateAnnouncementAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Announcement deactivated successfully"));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetAnnouncements()
    {
        var response =
            await _announcementService
                .GetAnnouncementsAsync();

        return Ok(
            ApiResponse<List<AnnouncementResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("active")]
    public async Task<IActionResult>
        GetActiveAnnouncements()
    {
        var response =
            await _announcementService
                .GetActiveAnnouncementsAsync();

        return Ok(
            ApiResponse<List<AnnouncementResponseDto>>
            .SuccessResponse(response));
    }
}
