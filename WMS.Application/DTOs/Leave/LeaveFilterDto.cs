using WMS.Application.Common.Models;

namespace WMS.Application.DTOs.Leave;

public class LeaveFilterDto : PaginationParams
{
    public int? EmployeeId { get; set; }

    public string? Status { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }
}
