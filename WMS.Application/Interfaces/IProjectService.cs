using WMS.Application.DTOs.Project;

namespace WMS.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectResponseDto>> GetMyProjectsAsync();

    Task<ProjectResponseDto> CreateProjectAsync(CreateProjectRequestDto request);

    Task UpdateProjectAsync(int projectId, UpdateProjectRequestDto request);

    Task<List<ProjectResponseDto>> GetProjectsAsync();

    Task<ProjectResponseDto> GetProjectByIdAsync(int projectId);

    Task AssignEmployeeAsync(int projectId, int employeeId);

    Task RemoveEmployeeAsync(int projectId, int employeeId);

    Task<List<ProjectAllocationDto>> GetProjectAllocationsAsync(int projectId);
}
