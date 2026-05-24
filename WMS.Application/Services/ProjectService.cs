using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Project;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUser;

    public ProjectService(
        IProjectRepository projectRepository,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _currentUser = currentUser;
    }

    public async Task<ProjectResponseDto>
        CreateProjectAsync(
            CreateProjectRequestDto request)
    {
        if (
            request.ClientId.HasValue
            &&
            !await _projectRepository
                .ClientExistsAsync(
                    request.ClientId.Value))
        {
            throw new BusinessRuleException(
                "Client does not exist");
        }

        var project = new Project
        {
            ProjectName = request.ProjectName,

            ClientId = request.ClientId,

            StartDate = request.StartDate,

            EndDate = request.EndDate,

            Status = Enum.Parse<ProjectStatus>(
                request.Status,
                true)
        };

        await _projectRepository
            .AddProjectAsync(project);

        return await GetProjectByIdAsync(
            project.ProjectId);
    }

    public async Task UpdateProjectAsync(
        int projectId,
        UpdateProjectRequestDto request)
    {
        var project =
            await _projectRepository
                .GetProjectByIdAsync(projectId)
            ??
            throw new NotFoundException(
                "Project not found");

        project.ProjectName =
            request.ProjectName;

        project.ClientId =
            request.ClientId;

        project.StartDate =
            request.StartDate;

        project.EndDate =
            request.EndDate;

        project.Status =
            Enum.Parse<ProjectStatus>(
                request.Status,
                true);

        await _projectRepository
            .UpdateProjectAsync(project);
    }

    public async Task<List<ProjectResponseDto>>
        GetProjectsAsync()
    {
        var projects =
            await _projectRepository
                .GetProjectsAsync();

        return projects
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<ProjectResponseDto>>
        GetMyProjectsAsync()
    {
        if (!_currentUser.EmployeeId.HasValue)
        {
            throw new BusinessRuleException(
                "Employee profile not found");
        }

        var projects =
            await _projectRepository
                .GetEmployeeProjectsAsync(
                    _currentUser.EmployeeId.Value);

        return projects
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProjectResponseDto>
        GetProjectByIdAsync(
            int projectId)
    {
        var project =
            await _projectRepository
                .GetProjectByIdAsync(projectId)
            ??
            throw new NotFoundException(
                "Project not found");

        return MapToDto(project);
    }

    public async Task AssignEmployeeAsync(
        int projectId,
        int employeeId)
    {
        var project =
            await _projectRepository
                .GetProjectByIdAsync(projectId)
            ??
            throw new NotFoundException(
                "Project not found");

        if (project.Status != ProjectStatus.Active)
        {
            throw new BusinessRuleException(
                "Employees can only be assigned to active projects");
        }

        bool employeeExists =
            await _projectRepository
                .EmployeeExistsAsync(employeeId);

        if (!employeeExists)
        {
            throw new BusinessRuleException(
                "Employee not found");
        }

        bool alreadyAssigned =
            await _projectRepository
                .AllocationExistsAsync(
                    projectId,
                    employeeId);

        if (alreadyAssigned)
        {
            throw new BusinessRuleException(
                "Employee already assigned to project");
        }

        var allocation =
            new EmployeeProject
            {
                EmpId = employeeId,

                ProjectId = projectId,

                AssignedOn =
                    DateOnly.FromDateTime(
                        DateTime.UtcNow),

                CreatedBy =
                    _currentUser.Username
            };

        await _projectRepository
            .AddAllocationAsync(allocation);
    }

    public async Task RemoveEmployeeAsync(
        int projectId,
        int employeeId)
    {
        var allocation =
            await _projectRepository
                .GetAllocationAsync(
                    projectId,
                    employeeId)
            ??
            throw new NotFoundException(
                "Project allocation not found");

        allocation.Status = false;

        allocation.UpdatedBy =
            _currentUser.Username;

        allocation.UpdatedDate =
            DateOnly.FromDateTime(
                DateTime.UtcNow);

        await _projectRepository
            .UpdateAllocationAsync(
                allocation);
    }

    public async Task<List<ProjectAllocationDto>>
        GetProjectAllocationsAsync(
            int projectId)
    {
        var allocations =
            await _projectRepository
                .GetProjectAllocationsAsync(
                    projectId);

        return allocations
            .Select(a =>
                new ProjectAllocationDto
                {
                    EmployeeId =
                        a.EmpId,

                    EmployeeName =
                        $"{a.Employee!.FirstName} {a.Employee!.LastName}",

                    Department =
                        a.Employee.Department?.DepartmentName
                        ?? "",

                    Role =
                        a.Employee.Role?.RoleName
                        ?? "",

                    AssignedOn =
                        a.AssignedOn,

                    Status =
                        a.Status
                })
            .ToList();
    }

    private static ProjectResponseDto
        MapToDto(Project project)
    {
        return new ProjectResponseDto
        {
            ProjectId =
                project.ProjectId,

            ProjectName =
                project.ProjectName,

            ClientId =
                project.ClientId,

            ClientName =
                project.Client?.ClientName,

            StartDate =
                project.StartDate,

            EndDate =
                project.EndDate,

            Status =
                project.Status.ToString(),

            TotalEmployees =
                project.EmployeeAllocations
                    .Count(a => a.Status)
        };
    }
}
