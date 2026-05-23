using WMS.Domain.Entities;
using WMS.Domain.Enums;

namespace WMS.Domain.Interfaces;

public interface ILeaveRepository
{
    Task AddLeaveAsync(Leave leave);

    Task UpdateLeaveAsync(Leave leave);

    Task<Leave?> GetByIdAsync(int leaveId);

    Task<List<Leave>> GetLeavesAsync(
        int? employeeId = null,
        LeaveStatus? status = null);

    Task<bool> HasOverlappingLeaveAsync(
        int employeeId,
        DateOnly fromDate,
        DateOnly toDate);
}
