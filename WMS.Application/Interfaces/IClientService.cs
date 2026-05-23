using WMS.Application.DTOs.Client;

namespace WMS.Application.Interfaces;

public interface IClientService
{
    Task<ClientResponseDto>
        CreateClientAsync(
            CreateClientRequestDto request);

    Task UpdateClientAsync(
        int clientId,
        UpdateClientRequestDto request);

    Task DeleteClientAsync(int clientId);

    Task<List<ClientResponseDto>>
        GetClientsAsync();

    Task<ClientResponseDto>
        GetClientByIdAsync(int clientId);

    Task<List<ClientProjectDto>>
        GetClientProjectsAsync(int clientId);
}
