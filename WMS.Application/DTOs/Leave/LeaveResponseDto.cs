namespace WMS.Application.DTOs.Leave;

public class LeaveResponseDto
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string LeaveType { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public int TotalDays { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime AppliedOn { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
}
