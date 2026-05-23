using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Leave;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepository;
    private readonly ICurrentUserService _currentUser;

    public LeaveService(
        ILeaveRepository leaveRepository,
        ICurrentUserService currentUser)
    {
        _leaveRepository = leaveRepository;
        _currentUser = currentUser;
    }

    public async Task ApplyLeaveAsync(
        ApplyLeaveRequestDto request)
    {
        int employeeId =
            _currentUser.EmployeeId
            ??
            throw new UnauthorizedException(
                "Employee account required");

        bool overlapping =
            await _leaveRepository
                .HasOverlappingLeaveAsync(
                    employeeId,
                    request.FromDate,
                    request.ToDate);

        if (overlapping)
        {
            throw new BusinessRuleException(
                "Overlapping leave already exists");
        }

        var leave = new Leave
        {
            EmpId = employeeId,

            LeaveType =
                Enum.Parse<LeaveType>(
                    request.LeaveType,
                    true),

            Reason = request.Reason,

            FromDate = request.FromDate,

            ToDate = request.ToDate,

            Status = LeaveStatus.Pending
        };

        await _leaveRepository
            .AddLeaveAsync(leave);
    }

    public async Task CancelLeaveAsync(
        int leaveId)
    {
        int employeeId =
            _currentUser.EmployeeId
            ??
            throw new UnauthorizedException(
                "Employee account required");

        var leave =
            await _leaveRepository
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            throw new NotFoundException(
                "Leave not found");
        }

        if (leave.EmpId != employeeId)
        {
            throw new UnauthorizedException(
                "Cannot cancel another employee's leave");
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            throw new BusinessRuleException(
                "Only pending leaves can be cancelled");
        }

        leave.Status = LeaveStatus.Cancelled;

        await _leaveRepository
            .UpdateLeaveAsync(leave);
    }

    public async Task ApproveLeaveAsync(
        int leaveId)
    {
        var leave =
            await _leaveRepository
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            throw new NotFoundException(
                "Leave not found");
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            throw new BusinessRuleException(
                "Only pending leaves can be approved");
        }

        leave.Status = LeaveStatus.Approved;

        leave.ApprovedBy =
            _currentUser.UserId;

        leave.ApprovedOn =
            DateTime.UtcNow;

        await _leaveRepository
            .UpdateLeaveAsync(leave);
    }

    public async Task RejectLeaveAsync(
        int leaveId)
    {
        var leave =
            await _leaveRepository
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            throw new NotFoundException(
                "Leave not found");
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            throw new BusinessRuleException(
                "Only pending leaves can be rejected");
        }

        leave.Status = LeaveStatus.Rejected;

        leave.ApprovedBy =
            _currentUser.UserId;

        leave.ApprovedOn =
            DateTime.UtcNow;

        await _leaveRepository
            .UpdateLeaveAsync(leave);
    }

    public async Task<List<LeaveResponseDto>>
        GetMyLeavesAsync()
    {
        int employeeId =
            _currentUser.EmployeeId
            ??
            throw new UnauthorizedException(
                "Employee account required");

        var leaves =
            await _leaveRepository
                .GetLeavesAsync(employeeId);

        return leaves
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<LeaveResponseDto>>
        GetLeavesAsync(
            LeaveFilterDto filter)
    {
        LeaveStatus? status = null;

        if (!string.IsNullOrWhiteSpace(
            filter.Status))
        {
            status =
                Enum.Parse<LeaveStatus>(
                    filter.Status,
                    true);
        }

        var leaves =
            await _leaveRepository
                .GetLeavesAsync(
                    filter.EmployeeId,
                    status);

        return leaves
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<LeaveResponseDto>>
        GetPendingLeavesAsync()
    {
        var leaves =
            await _leaveRepository
                .GetLeavesAsync(
                    null,
                    LeaveStatus.Pending);

        return leaves
            .Select(MapToDto)
            .ToList();
    }

    private static LeaveResponseDto
        MapToDto(Leave leave)
    {
        return new LeaveResponseDto
        {
            LeaveId = leave.LeaveId,

            EmployeeId = leave.EmpId,

            EmployeeName =
                $"{leave.Employee?.FirstName} {leave.Employee?.LastName}",

            LeaveType =
                leave.LeaveType.ToString(),

            Reason = leave.Reason,

            FromDate = leave.FromDate,

            ToDate = leave.ToDate,

            TotalDays =
                leave.ToDate.DayNumber
                - leave.FromDate.DayNumber
                + 1,

            Status =
                leave.Status.ToString(),

            AppliedOn =
                leave.AppliedOn,

            ApprovedBy =
                leave.ApprovedBy,

            ApprovedOn =
                leave.ApprovedOn
        };
    }
}
