using WMS.Application.Common.Models;
using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Employee;

public class EmployeeFilterDto : PaginationParams
{
    public string? Search { get; set; }

    public int? DepartmentId { get; set; }

    public int? RoleId { get; set; }

    public EmployeeStatus? Status { get; set; }

    public string? SortBy { get; set; } = "firstname";

    public string? SortDirection { get; set; } = "asc";
}
