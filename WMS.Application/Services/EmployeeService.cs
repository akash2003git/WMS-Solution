using WMS.Application.Common.Exceptions;
using WMS.Application.Common.Models;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using WMS.Application.Common.Constants;

namespace WMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICurrentUserService _currentUserService;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        ICurrentUserService currentUserService)
    {
        _employeeRepository = employeeRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(
        CreateEmployeeRequestDto request)
    {
        var normalized =
            NormalizeManagerCreateRequest(
                request.DepartmentId,
                request.RoleId);

        await ValidateEmployeeAsync(
            request.Email,
            normalized.DepartmentId,
            normalized.RoleId);

        string tempPassword = GenerateTemporaryPassword();

        string passwordHash =
            BCrypt.Net.BCrypt.HashPassword(tempPassword);

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            DOB = request.DOB,
            DOJ = request.DOJ,
            DepartmentId = normalized.DepartmentId,
            RoleId = normalized.RoleId
        };

        var userLogin = new UserLogin
        {
            Username = request.Email,
            PasswordHash = passwordHash,
            RoleId = normalized.RoleId,
            MustChangePassword = true
        };

        await _employeeRepository
            .CreateEmployeeAsync(employee, userLogin);

        return new CreateEmployeeResponseDto
        {
            EmployeeId = employee.EmployeeId,
            Username = userLogin.Username,
            TemporaryPassword = tempPassword
        };
    }

    public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(
        int employeeId)
    {
        var employee = await _employeeRepository
            .GetByIdAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        return MapToDto(employee);
    }

    public async Task<PagedResponse<EmployeeResponseDto>> GetEmployeesAsync(
        EmployeeFilterDto filter)
    {
        var employees = await _employeeRepository
            .GetEmployeesAsync();

        IQueryable<Employee> query = employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(e =>
                e.FirstName.Contains(
                    filter.Search,
                    StringComparison.OrdinalIgnoreCase)
                ||
                e.LastName.Contains(
                    filter.Search,
                    StringComparison.OrdinalIgnoreCase)
                ||
                e.Email.Contains(
                    filter.Search,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(e =>
                e.DepartmentId == filter.DepartmentId.Value);
        }

        if (filter.RoleId.HasValue)
        {
            query = query.Where(e =>
                e.RoleId == filter.RoleId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(e =>
                e.Status == filter.Status.Value);
        }

        query = ApplySorting(
            query,
            filter.SortBy,
            filter.SortDirection);

        int totalCount = query.Count();

        var items = query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResponse<EmployeeResponseDto>
        {
            Items = items,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task UpdateEmployeeAsync(
        int employeeId,
        UpdateEmployeeRequestDto request)
    {
        var employee = await _employeeRepository
            .GetByIdAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        EnsureCanModifyEmployee(employee);

        var existingEmployee =
            await _employeeRepository
                .GetEmployeeByEmailAsync(request.Email);

        if (existingEmployee != null &&
            existingEmployee.EmployeeId != employeeId)
        {
            throw new BusinessRuleException(
                "Email already exists");
        }

        EnsureManagerCannotChangeRestrictedFields(
            employee,
            request);

        await ValidateDepartmentAndRole(
            request.DepartmentId,
            request.RoleId);

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Gender = request.Gender;
        employee.DOB = request.DOB;
        employee.DOJ = request.DOJ;
        employee.DepartmentId = request.DepartmentId;
        employee.RoleId = request.RoleId;
        employee.UpdatedOn = DateTime.UtcNow;

        await _employeeRepository
            .UpdateEmployeeAsync(employee);
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        var employee = await _employeeRepository
            .GetByIdAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        EnsureCanModifyEmployee(employee);

        employee.Status = EmployeeStatus.Inactive;

        employee.UpdatedOn = DateTime.UtcNow;

        await _employeeRepository.SaveChangesAsync();
    }

    private void EnsureCanModifyEmployee(Employee employee)
    {
        if (_currentUserService.Role == "Admin")
        {
            return;
        }

        if (_currentUserService.Role == "Manager")
        {
            if (_currentUserService.DepartmentId ==
                employee.DepartmentId)
            {
                return;
            }

            throw new ForbiddenException(
                "You cannot modify employees outside your department");
        }

        throw new ForbiddenException(
            "You do not have permission to modify employees");
    }

    private (
        int DepartmentId,
        int RoleId)
    NormalizeManagerCreateRequest(
        int departmentId,
        int roleId)
    {
        if (_currentUserService.Role != "Manager")
        {
            return (departmentId, roleId);
        }

        return (
            _currentUserService.DepartmentId!.Value,
            SystemRoles.Employee
        );
    }

    private void EnsureManagerCannotChangeRestrictedFields(
        Employee employee,
        UpdateEmployeeRequestDto request)
    {
        if (_currentUserService.Role != "Manager")
        {
            return;
        }

        if (request.DepartmentId != employee.DepartmentId)
        {
            throw new ForbiddenException(
                "Managers cannot change employee department");
        }

        if (request.RoleId != employee.RoleId)
        {
            throw new ForbiddenException(
                "Managers cannot change employee role");
        }
    }

    private async Task ValidateEmployeeAsync(
        string email,
        int departmentId,
        int roleId)
    {
        bool exists = await _employeeRepository
            .UsernameExistsAsync(email);

        if (exists)
        {
            throw new BusinessRuleException(
                "Email already exists");
        }

        await ValidateDepartmentAndRole(
            departmentId,
            roleId);
    }

    private async Task ValidateDepartmentAndRole(
        int departmentId,
        int roleId)
    {
        bool departmentExists =
            await _employeeRepository
                .DepartmentExistsAsync(departmentId);

        if (!departmentExists)
        {
            throw new BusinessRuleException(
                "Invalid department");
        }

        bool roleExists =
            await _employeeRepository
                .RoleExistsAsync(roleId);

        if (!roleExists)
        {
            throw new BusinessRuleException(
                "Invalid role");
        }
    }

    private static IQueryable<Employee> ApplySorting(
        IQueryable<Employee> query,
        string? sortBy,
        string? direction)
    {
        bool desc = direction?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "lastname" =>
                desc
                    ? query.OrderByDescending(e => e.LastName)
                    : query.OrderBy(e => e.LastName),

            "email" =>
                desc
                    ? query.OrderByDescending(e => e.Email)
                    : query.OrderBy(e => e.Email),

            "doj" =>
                desc
                    ? query.OrderByDescending(e => e.DOJ)
                    : query.OrderBy(e => e.DOJ),

            _ =>
                desc
                    ? query.OrderByDescending(e => e.FirstName)
                    : query.OrderBy(e => e.FirstName)
        };
    }

    private static EmployeeResponseDto MapToDto(
        Employee employee)
    {
        return new EmployeeResponseDto
        {
            EmployeeId = employee.EmployeeId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Gender = employee.Gender,
            DOB = employee.DOB,
            DOJ = employee.DOJ,
            DepartmentId = employee.DepartmentId,
            DepartmentName =
                employee.Department?.DepartmentName ?? "",
            RoleId = employee.RoleId,
            RoleName =
                employee.Role?.RoleName ?? "",
            Status = employee.Status.ToString()
        };
    }

    private static string GenerateTemporaryPassword()
    {
        return $"Temp@{Random.Shared.Next(1000, 9999)}";
    }
}
