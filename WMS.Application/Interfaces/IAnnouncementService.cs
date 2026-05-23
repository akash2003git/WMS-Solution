using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Interfaces;

public interface IAnnouncementService
{
    Task<AnnouncementResponseDto>
        CreateAnnouncementAsync(
            CreateAnnouncementRequestDto request);

    Task UpdateAnnouncementAsync(
        int announcementId,
        UpdateAnnouncementRequestDto request);

    Task ActivateAnnouncementAsync(
        int announcementId);

    Task DeactivateAnnouncementAsync(
        int announcementId);

    Task<List<AnnouncementResponseDto>>
        GetAnnouncementsAsync();

    Task<List<AnnouncementResponseDto>>
        GetActiveAnnouncementsAsync();
}
