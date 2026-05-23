using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class AnnouncementRepository
    : IAnnouncementRepository
{
    private readonly WmsDbContext _context;

    public AnnouncementRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<Announcement>
        AddAnnouncementAsync(
            Announcement announcement)
    {
        await _context.Announcements
            .AddAsync(announcement);

        await _context.SaveChangesAsync();

        return announcement;
    }

    public async Task UpdateAnnouncementAsync(
        Announcement announcement)
    {
        _context.Announcements
            .Update(announcement);

        await _context.SaveChangesAsync();
    }

    public async Task<Announcement?>
        GetByIdAsync(int announcementId)
    {
        return await _context.Announcements
            .FirstOrDefaultAsync(a =>
                a.AnnouncementId == announcementId);
    }

    public async Task<List<Announcement>>
        GetAnnouncementsAsync()
    {
        return await _context.Announcements
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();
    }

    public async Task<List<Announcement>>
        GetActiveAnnouncementsAsync()
    {
        return await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();
    }
}
