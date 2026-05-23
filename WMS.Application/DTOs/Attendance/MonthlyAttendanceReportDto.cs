namespace WMS.Application.DTOs.Attendance;

public class MonthlyAttendanceReportDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Month { get; set; }

    public int TotalPresentDays { get; set; }

    public int TotalAbsentDays { get; set; }

    public double TotalHoursWorked { get; set; }

    public double AverageHoursPerDay { get; set; }

    public double AttendancePercentage { get; set; }
}
