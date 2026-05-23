using WMS.Application.DTOs.Attendance;

namespace WMS.Application.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceResponseDto> CheckInAsync(AttendanceRequestDto request);
    Task<AttendanceResponseDto> CheckOutAsync();
    Task<List<AttendanceResponseDto>> GetMyAttendanceAsync(AttendanceFilterDto filter);
    Task<MonthlyAttendanceReportDto>
        GetMonthlyReportAsync(int? employeeId = null);
    Task<List<AttendanceResponseDto>>
        GetAttendanceHistoryAsync(AttendanceFilterDto filter);
    Task<List<AbsenteeDto>> GetAbsenteesAsync();
}
