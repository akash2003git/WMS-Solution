using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly WmsDbContext _context;

    public ClientRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<Client> AddClientAsync(Client client)
    {
        await _context.Clients.AddAsync(client);

        await _context.SaveChangesAsync();

        return client;
    }

    public async Task UpdateClientAsync(Client client)
    {
        _context.Clients.Update(client);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteClientAsync(Client client)
    {
        _context.Clients.Remove(client);

        await _context.SaveChangesAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int clientId)
    {
        return await _context.Clients
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c =>
                c.ClientId == clientId);
    }

    public async Task<List<Client>> GetClientsAsync()
    {
        return await _context.Clients
            .Include(c => c.Projects)
            .ToListAsync();
    }

    public async Task<bool> ClientExistsAsync(int clientId)
    {
        return await _context.Clients
            .AnyAsync(c => c.ClientId == clientId);
    }

    public async Task<bool> HasProjectsAsync(int clientId)
    {
        return await _context.Projects
            .AnyAsync(p => p.ClientId == clientId);
    }

    public async Task<List<Project>>
        GetClientProjectsAsync(int clientId)
    {
        return await _context.Projects
            .Include(p => p.EmployeeAllocations)
            .Where(p => p.ClientId == clientId)
            .ToListAsync();
    }
}
