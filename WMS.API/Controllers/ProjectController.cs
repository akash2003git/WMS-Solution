using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Project;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(
        IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("my-projects")]
    public async Task<IActionResult>
        GetMyProjects()
    {
        var response =
            await _projectService
                .GetMyProjectsAsync();

        return Ok(
            ApiResponse<List<ProjectResponseDto>>
            .SuccessResponse(response));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        CreateProject(
            CreateProjectRequestDto request)
    {
        var response =
            await _projectService
                .CreateProjectAsync(request);

        return Ok(
            ApiResponse<ProjectResponseDto>
            .SuccessResponse(
                response,
                "Project created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        UpdateProject(
            int id,
            UpdateProjectRequestDto request)
    {
        await _projectService
            .UpdateProjectAsync(
                id,
                request);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Project updated successfully"));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetProjects()
    {
        var response =
            await _projectService
                .GetProjectsAsync();

        return Ok(
            ApiResponse<List<ProjectResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetProjectById(int id)
    {
        var response =
            await _projectService
                .GetProjectByIdAsync(id);

        return Ok(
            ApiResponse<ProjectResponseDto>
            .SuccessResponse(response));
    }

    [HttpPost("{id}/assign-employee")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        AssignEmployee(
            int id,
            AssignEmployeeDto request)
    {
        await _projectService
            .AssignEmployeeAsync(
                id,
                request.EmployeeId);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Employee assigned successfully"));
    }

    [HttpDelete("{id}/remove-employee/{employeeId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        RemoveEmployee(
            int id,
            int employeeId)
    {
        await _projectService
            .RemoveEmployeeAsync(
                id,
                employeeId);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Employee removed successfully"));
    }

    [HttpGet("{id}/allocations")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetProjectAllocations(int id)
    {
        var response =
            await _projectService
                .GetProjectAllocationsAsync(id);

        return Ok(
            ApiResponse<List<ProjectAllocationDto>>
            .SuccessResponse(response));
    }
}
