using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAnnouncementRepository
{
    Task<Announcement>
        AddAnnouncementAsync(
            Announcement announcement);

    Task UpdateAnnouncementAsync(
        Announcement announcement);

    Task<Announcement?>
        GetByIdAsync(int announcementId);

    Task<List<Announcement>>
        GetAnnouncementsAsync();

    Task<List<Announcement>>
        GetActiveAnnouncementsAsync();
}
