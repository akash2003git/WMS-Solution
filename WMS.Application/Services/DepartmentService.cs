using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Department;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<List<DepartmentResponseDto>>
        GetDepartmentsAsync()
    {
        var departments =
            await _departmentRepository
                .GetDepartmentsAsync();

        return departments
            .Select(MapToDto)
            .ToList();
    }

    public async Task<DepartmentResponseDto>
        GetDepartmentByIdAsync(int departmentId)
    {
        var department =
            await _departmentRepository
                .GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new NotFoundException(
                "Department not found");
        }

        return MapToDto(department);
    }

    public async Task<List<DepartmentEmployeeDto>>
        GetDepartmentEmployeesAsync(int departmentId)
    {
        var department =
            await _departmentRepository
                .GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new NotFoundException(
                "Department not found");
        }

        return department.Employees
            .Select(e => new DepartmentEmployeeDto
            {
                EmployeeId = e.EmployeeId,
                FullName =
                    $"{e.FirstName} {e.LastName}",
                Email = e.Email,
                Role = e.Role?.RoleName ?? "",
                Status = e.Status.ToString()
            })
            .ToList();
    }

    public async Task CreateDepartmentAsync(
        CreateDepartmentRequestDto request)
    {
        bool exists =
            await _departmentRepository
                .DepartmentNameExistsAsync(
                    request.DepartmentName);

        if (exists)
        {
            throw new BusinessRuleException(
                "Department already exists");
        }

        var department = new Department
        {
            DepartmentName = request.DepartmentName,
            Description = request.Description
        };

        await _departmentRepository
            .CreateDepartmentAsync(department);
    }

    public async Task UpdateDepartmentAsync(
        int departmentId,
        UpdateDepartmentRequestDto request)
    {
        var department =
            await _departmentRepository
                .GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new NotFoundException(
                "Department not found");
        }

        bool exists =
            await _departmentRepository
                .DepartmentNameExistsAsync(
                    request.DepartmentName,
                    departmentId);

        if (exists)
        {
            throw new BusinessRuleException(
                "Department already exists");
        }

        department.DepartmentName =
            request.DepartmentName;

        department.Description =
            request.Description;

        await _departmentRepository
            .UpdateDepartmentAsync(department);
    }

    public async Task DeleteDepartmentAsync(
        int departmentId)
    {
        var department =
            await _departmentRepository
                .GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new NotFoundException(
                "Department not found");
        }

        bool hasEmployees =
            await _departmentRepository
                .HasEmployeesAsync(departmentId);

        if (hasEmployees)
        {
            throw new BusinessRuleException(
                "Cannot delete department with employees");
        }

        await _departmentRepository
            .DeleteDepartmentAsync(department);
    }

    private static DepartmentResponseDto MapToDto(
        Department department)
    {
        return new DepartmentResponseDto
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName,
            Description = department.Description,
            EmployeeCount = department.Employees.Count
        };
    }
}
