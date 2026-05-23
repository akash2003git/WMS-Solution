using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAttendanceRepository
{
    Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateOnly date);
    Task AddAttendanceAsync(Attendance attendance);
    Task UpdateAttendanceAsync(Attendance attendance);
    Task<List<Attendance>> GetAttendancesAsync(
        int? employeeId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null);
    Task<List<Employee>> GetAbsenteesAsync(DateOnly date);
}
