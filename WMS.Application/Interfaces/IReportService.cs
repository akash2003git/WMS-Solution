using WMS.Application.DTOs.Attendance;
using WMS.Application.DTOs.Report;
using WMS.Domain.Enums;

namespace WMS.Application.Interfaces;

public interface IReportService
{
    Task<MonthlyAttendanceReportDto>
        GetMonthlyAttendanceReportAsync(
            int employeeId,
            int year,
            int month);

    Task<List<AttendanceResponseDto>>
        GetAttendanceHistoryAsync(int employeeId);

    Task<List<LeaveReportDto>>
        GetLeaveReportAsync(
            int? employeeId,
            LeaveStatus? status);

    Task<List<DepartmentEmployeeReportDto>>
        GetDepartmentEmployeeReportAsync(int departmentId);

    Task<List<ProjectAllocationReportDto>>
        GetProjectAllocationReportAsync();
}
