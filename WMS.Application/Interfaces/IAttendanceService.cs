using WMS.Application.DTOs.Attendance;
using WMS.Application.Common.Models;

namespace WMS.Application.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceResponseDto> CheckInAsync(AttendanceRequestDto request);
    Task<AttendanceResponseDto> CheckOutAsync();
    Task<TodayAttendanceDto> GetTodayAttendanceAsync();
    Task<PagedResponse<AttendanceResponseDto>>
        GetMyAttendanceAsync(AttendanceFilterDto filter);
    Task<MonthlyAttendanceReportDto>
        GetMonthlyReportAsync(int? employeeId = null);
    Task<PagedResponse<AttendanceResponseDto>>
        GetAttendanceHistoryAsync(AttendanceFilterDto filter);
    Task<List<AbsenteeDto>> GetAbsenteesAsync();
}
