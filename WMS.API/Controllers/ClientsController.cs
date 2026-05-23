using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Client;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(
        IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost]
    public async Task<IActionResult>
        CreateClient(
            CreateClientRequestDto request)
    {
        var response =
            await _clientService
                .CreateClientAsync(request);

        return Ok(
            ApiResponse<ClientResponseDto>
            .SuccessResponse(
                response,
                "Client created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>
        UpdateClient(
            int id,
            UpdateClientRequestDto request)
    {
        await _clientService
            .UpdateClientAsync(id, request);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Client updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteClient(int id)
    {
        await _clientService
            .DeleteClientAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Client deleted successfully"));
    }

    [HttpGet]
    public async Task<IActionResult>
        GetClients()
    {
        var response =
            await _clientService
                .GetClientsAsync();

        return Ok(
            ApiResponse<List<ClientResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>
        GetClientById(int id)
    {
        var response =
            await _clientService
                .GetClientByIdAsync(id);

        return Ok(
            ApiResponse<ClientResponseDto>
            .SuccessResponse(response));
    }

    [HttpGet("{id}/projects")]
    public async Task<IActionResult>
        GetClientProjects(int id)
    {
        var response =
            await _clientService
                .GetClientProjectsAsync(id);

        return Ok(
            ApiResponse<List<ClientProjectDto>>
            .SuccessResponse(response));
    }
}
