namespace WMS.Application.DTOs.Attendance;

public class TodayAttendanceDto
{
    public bool HasCheckedIn { get; set; }

    public bool HasCheckedOut { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public double? TotalHours { get; set; }

    public string? WorkMode { get; set; }
}
