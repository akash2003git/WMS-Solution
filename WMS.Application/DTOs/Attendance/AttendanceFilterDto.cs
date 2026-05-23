using WMS.Application.Common.Models;

namespace WMS.Application.DTOs.Attendance;

public class AttendanceFilterDto : PaginationParams
{
    public int? EmployeeId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}
