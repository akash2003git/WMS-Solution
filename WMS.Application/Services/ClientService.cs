using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Client;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(
        IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientResponseDto>
        CreateClientAsync(
            CreateClientRequestDto request)
    {
        var client = new Client
        {
            ClientName = request.ClientName,
            ClientAddress = request.ClientAddress,
            ClientPhoneNumber = request.ClientPhoneNumber,
            ClientLocation = request.ClientLocation
        };

        await _clientRepository
            .AddClientAsync(client);

        return MapToDto(client);
    }

    public async Task UpdateClientAsync(
        int clientId,
        UpdateClientRequestDto request)
    {
        var client =
            await _clientRepository
                .GetClientByIdAsync(clientId)
            ??
            throw new NotFoundException(
                "Client not found");

        client.ClientName = request.ClientName;

        client.ClientAddress = request.ClientAddress;

        client.ClientPhoneNumber =
            request.ClientPhoneNumber;

        client.ClientLocation =
            request.ClientLocation;

        client.Status = request.Status;

        await _clientRepository
            .UpdateClientAsync(client);
    }

    public async Task DeleteClientAsync(int clientId)
    {
        var client =
            await _clientRepository
                .GetClientByIdAsync(clientId)
            ??
            throw new NotFoundException(
                "Client not found");

        bool hasProjects =
            await _clientRepository
                .HasProjectsAsync(clientId);

        if (hasProjects)
        {
            throw new BusinessRuleException(
                "Cannot delete client with projects");
        }

        await _clientRepository
            .DeleteClientAsync(client);
    }

    public async Task<List<ClientResponseDto>>
        GetClientsAsync()
    {
        var clients =
            await _clientRepository
                .GetClientsAsync();

        return clients
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ClientResponseDto>
        GetClientByIdAsync(int clientId)
    {
        var client =
            await _clientRepository
                .GetClientByIdAsync(clientId)
            ??
            throw new NotFoundException(
                "Client not found");

        return MapToDto(client);
    }

    public async Task<List<ClientProjectDto>>
        GetClientProjectsAsync(int clientId)
    {
        bool exists =
            await _clientRepository
                .ClientExistsAsync(clientId);

        if (!exists)
        {
            throw new NotFoundException(
                "Client not found");
        }

        var projects =
            await _clientRepository
                .GetClientProjectsAsync(clientId);

        return projects
            .Select(p =>
                new ClientProjectDto
                {
                    ProjectId = p.ProjectId,

                    ProjectName = p.ProjectName,

                    Status = p.Status.ToString(),

                    StartDate = p.StartDate,

                    EndDate = p.EndDate,

                    TotalEmployees =
                        p.EmployeeAllocations
                            .Count(a => a.Status)
                })
            .ToList();
    }

    private static ClientResponseDto
        MapToDto(Client client)
    {
        return new ClientResponseDto
        {
            ClientId = client.ClientId,

            ClientName = client.ClientName,

            ClientAddress = client.ClientAddress,

            ClientPhoneNumber =
                client.ClientPhoneNumber,

            ClientLocation =
                client.ClientLocation,

            Status = client.Status,

            TotalProjects =
                client.Projects.Count
        };
    }
}
