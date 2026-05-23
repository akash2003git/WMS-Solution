using WMS.Application.DTOs.Leave;

namespace WMS.Application.Interfaces;

public interface ILeaveService
{
    Task ApplyLeaveAsync(
        ApplyLeaveRequestDto request);

    Task CancelLeaveAsync(int leaveId);

    Task ApproveLeaveAsync(int leaveId);

    Task RejectLeaveAsync(int leaveId);

    Task<List<LeaveResponseDto>>
        GetMyLeavesAsync();

    Task<List<LeaveResponseDto>>
        GetLeavesAsync(LeaveFilterDto filter);

    Task<List<LeaveResponseDto>>
        GetPendingLeavesAsync();
}
