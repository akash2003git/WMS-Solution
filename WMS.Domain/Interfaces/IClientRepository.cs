using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client> AddClientAsync(Client client);

    Task UpdateClientAsync(Client client);

    Task DeleteClientAsync(Client client);

    Task<Client?> GetClientByIdAsync(int clientId);

    Task<List<Client>> GetClientsAsync();

    Task<bool> ClientExistsAsync(int clientId);

    Task<bool> HasProjectsAsync(int clientId);

    Task<List<Project>>
        GetClientProjectsAsync(int clientId);
}
