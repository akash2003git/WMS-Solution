using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly WmsDbContext _context;

    public ProjectRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<Project> AddProjectAsync(Project project)
    {
        await _context.Projects.AddAsync(project);

        await _context.SaveChangesAsync();

        return project;
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);

        await _context.SaveChangesAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.EmployeeAllocations)
            .FirstOrDefaultAsync(p =>
                p.ProjectId == projectId);
    }

    public async Task<List<Project>>
        GetEmployeeProjectsAsync(int employeeId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Project)
                .ThenInclude(p => p!.Client)
            .Include(ep => ep.Project)
                .ThenInclude(p => p!.EmployeeAllocations)
            .Where(ep =>
                ep.EmpId == employeeId
                &&
                ep.Status)
            .Select(ep => ep.Project!)
            .ToListAsync();
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.EmployeeAllocations)
            .ToListAsync();
    }

    public async Task<bool> ProjectExistsAsync(int projectId)
    {
        return await _context.Projects
            .AnyAsync(p => p.ProjectId == projectId);
    }

    public async Task<bool> ClientExistsAsync(int clientId)
    {
        return await _context.Clients
            .AnyAsync(c => c.ClientId == clientId);
    }

    public async Task<bool> EmployeeExistsAsync(int employeeId)
    {
        return await _context.Employees
            .AnyAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<bool> AllocationExistsAsync(
        int projectId,
        int employeeId)
    {
        return await _context.EmployeeProjects
            .AnyAsync(ep =>
                ep.ProjectId == projectId
                &&
                ep.EmpId == employeeId
                &&
                ep.Status);
    }

    public async Task AddAllocationAsync(
        EmployeeProject allocation)
    {
        await _context.EmployeeProjects
            .AddAsync(allocation);

        await _context.SaveChangesAsync();
    }

    public async Task<EmployeeProject?> GetAllocationAsync(
        int projectId,
        int employeeId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Employee)
                .ThenInclude(e => e!.Department)
            .Include(ep => ep.Employee)
                .ThenInclude(e => e!.Role)
            .FirstOrDefaultAsync(ep =>
                ep.ProjectId == projectId
                &&
                ep.EmpId == employeeId
                &&
                ep.Status);
    }

    public async Task<List<EmployeeProject>>
        GetProjectAllocationsAsync(int projectId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Employee)
                .ThenInclude(e => e!.Department)
            .Include(ep => ep.Employee)
                .ThenInclude(e => e!.Role)
            .Where(ep =>
                ep.ProjectId == projectId
                &&
                ep.Status)
            .ToListAsync();
    }

    public async Task UpdateAllocationAsync(
        EmployeeProject allocation)
    {
        _context.EmployeeProjects
            .Update(allocation);

        await _context.SaveChangesAsync();
    }
}
