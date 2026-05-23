namespace WMS.Application.DTOs.Report;

public class LeaveReportDto
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string LeaveType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public DateTime AppliedOn { get; set; }
}
