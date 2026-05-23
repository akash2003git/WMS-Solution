namespace WMS.Application.DTOs.Attendance;

public class AttendanceRequestDto
{
    public int? EmpId { get; set; }
    public string WorkMode { get; set; } = "WFO"; // WFO, WFH, Hybrid
}
