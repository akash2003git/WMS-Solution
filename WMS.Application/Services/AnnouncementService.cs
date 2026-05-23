using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AnnouncementService
    : IAnnouncementService
{
    private readonly IAnnouncementRepository
        _announcementRepository;

    private readonly ICurrentUserService
        _currentUser;

    public AnnouncementService(
        IAnnouncementRepository announcementRepository,
        ICurrentUserService currentUser)
    {
        _announcementRepository =
            announcementRepository;

        _currentUser = currentUser;
    }

    public async Task<AnnouncementResponseDto>
        CreateAnnouncementAsync(
            CreateAnnouncementRequestDto request)
    {
        var announcement = new Announcement
        {
            Title = request.Title,

            Message = request.Message,

            CreatedBy = _currentUser.UserId,

            IsActive = true
        };

        await _announcementRepository
            .AddAnnouncementAsync(
                announcement);

        return MapToDto(announcement);
    }

    public async Task UpdateAnnouncementAsync(
        int announcementId,
        UpdateAnnouncementRequestDto request)
    {
        var announcement =
            await _announcementRepository
                .GetByIdAsync(announcementId)
            ??
            throw new NotFoundException(
                "Announcement not found");

        announcement.Title = request.Title;

        announcement.Message = request.Message;

        await _announcementRepository
            .UpdateAnnouncementAsync(
                announcement);
    }

    public async Task ActivateAnnouncementAsync(
        int announcementId)
    {
        var announcement =
            await _announcementRepository
                .GetByIdAsync(announcementId)
            ??
            throw new NotFoundException(
                "Announcement not found");

        announcement.IsActive = true;

        await _announcementRepository
            .UpdateAnnouncementAsync(
                announcement);
    }

    public async Task DeactivateAnnouncementAsync(
        int announcementId)
    {
        var announcement =
            await _announcementRepository
                .GetByIdAsync(announcementId)
            ??
            throw new NotFoundException(
                "Announcement not found");

        announcement.IsActive = false;

        await _announcementRepository
            .UpdateAnnouncementAsync(
                announcement);
    }

    public async Task<List<AnnouncementResponseDto>>
        GetAnnouncementsAsync()
    {
        var announcements =
            await _announcementRepository
                .GetAnnouncementsAsync();

        return announcements
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<AnnouncementResponseDto>>
        GetActiveAnnouncementsAsync()
    {
        var announcements =
            await _announcementRepository
                .GetActiveAnnouncementsAsync();

        return announcements
            .Select(MapToDto)
            .ToList();
    }

    private static AnnouncementResponseDto
        MapToDto(Announcement announcement)
    {
        return new AnnouncementResponseDto
        {
            AnnouncementId =
                announcement.AnnouncementId,

            Title =
                announcement.Title,

            Message =
                announcement.Message,

            CreatedBy =
                announcement.CreatedBy,

            CreatedOn =
                announcement.CreatedOn,

            IsActive =
                announcement.IsActive
        };
    }
}
