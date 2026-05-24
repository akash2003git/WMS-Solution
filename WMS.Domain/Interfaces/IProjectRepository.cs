using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IProjectRepository
{
    Task<Project> AddProjectAsync(Project project);

    Task UpdateProjectAsync(Project project);

    Task<List<Project>> GetEmployeeProjectsAsync(int employeeId);

    Task<Project?> GetProjectByIdAsync(int projectId);

    Task<List<Project>> GetProjectsAsync();

    Task<bool> ProjectExistsAsync(int projectId);

    Task<bool> ClientExistsAsync(int clientId);

    Task<bool> EmployeeExistsAsync(int employeeId);

    Task<bool> AllocationExistsAsync(int projectId, int employeeId);

    Task AddAllocationAsync(EmployeeProject allocation);

    Task<EmployeeProject?> GetAllocationAsync(int projectId, int employeeId);

    Task<List<EmployeeProject>> GetProjectAllocationsAsync(int projectId);

    Task UpdateAllocationAsync(EmployeeProject allocation);
}
