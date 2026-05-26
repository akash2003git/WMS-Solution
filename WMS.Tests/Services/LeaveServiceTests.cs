using FluentAssertions;
using Moq;
using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Leave;
using WMS.Application.Interfaces;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class LeaveServiceTests
{
    private readonly Mock<ILeaveRepository>
        _leaveRepositoryMock;

    private readonly Mock<ICurrentUserService>
        _currentUserMock;

    private readonly LeaveService _leaveService;

    public LeaveServiceTests()
    {
        _leaveRepositoryMock =
            new Mock<ILeaveRepository>();

        _currentUserMock =
            new Mock<ICurrentUserService>();

        _leaveService =
            new LeaveService(
                _leaveRepositoryMock.Object,
                _currentUserMock.Object);
    }

    [Fact]
    public async Task ApplyLeave_ValidRequest_CreatesLeave()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var request = new ApplyLeaveRequestDto
        {
            LeaveType = "Sick",
            Reason = "Fever",
            FromDate = new DateOnly(2026, 5, 1),
            ToDate = new DateOnly(2026, 5, 3)
        };

        _leaveRepositoryMock
            .Setup(x =>
                x.HasOverlappingLeaveAsync(
                    1,
                    request.FromDate,
                    request.ToDate))
            .ReturnsAsync(false);

        // Act

        await _leaveService
            .ApplyLeaveAsync(request);

        // Assert

        _leaveRepositoryMock.Verify(
            x => x.AddLeaveAsync(
                It.Is<Leave>(l =>
                    l.EmpId == 1
                    &&
                    l.LeaveType == LeaveType.Sick
                    &&
                    l.Status == LeaveStatus.Pending)),
            Times.Once);
    }

    [Fact]
    public async Task ApplyLeave_OverlappingLeave_ThrowsBusinessRuleException()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var request = new ApplyLeaveRequestDto
        {
            LeaveType = "Casual",
            Reason = "Vacation",
            FromDate = new DateOnly(2026, 5, 1),
            ToDate = new DateOnly(2026, 5, 5)
        };

        _leaveRepositoryMock
            .Setup(x =>
                x.HasOverlappingLeaveAsync(
                    1,
                    request.FromDate,
                    request.ToDate))
            .ReturnsAsync(true);

        // Act

        Func<Task> action =
            async () =>
                await _leaveService
                    .ApplyLeaveAsync(request);

        // Assert

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage(
                "Overlapping leave already exists");

        _leaveRepositoryMock.Verify(
            x => x.AddLeaveAsync(It.IsAny<Leave>()),
            Times.Never);
    }

    [Fact]
    public async Task ApproveLeave_PendingLeave_UpdatesStatus()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.UserId)
            .Returns(99);

        var leave = new Leave
        {
            LeaveId = 1,
            EmpId = 1,
            Status = LeaveStatus.Pending
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        // Act

        await _leaveService
            .ApproveLeaveAsync(1);

        // Assert

        leave.Status.Should()
            .Be(LeaveStatus.Approved);

        leave.ApprovedBy.Should()
            .Be(99);

        leave.ApprovedOn.Should()
            .NotBeNull();

        _leaveRepositoryMock.Verify(
            x => x.UpdateLeaveAsync(leave),
            Times.Once);
    }

    [Fact]
    public async Task RejectLeave_PendingLeave_UpdatesStatus()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.UserId)
            .Returns(50);

        var leave = new Leave
        {
            LeaveId = 1,
            EmpId = 1,
            Status = LeaveStatus.Pending
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        // Act

        await _leaveService
            .RejectLeaveAsync(1);

        // Assert

        leave.Status.Should()
            .Be(LeaveStatus.Rejected);

        leave.ApprovedBy.Should()
            .Be(50);

        leave.ApprovedOn.Should()
            .NotBeNull();

        _leaveRepositoryMock.Verify(
            x => x.UpdateLeaveAsync(leave),
            Times.Once);
    }

    [Fact]
    public async Task CancelLeave_ApprovedLeave_ThrowsBusinessRuleException()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var leave = new Leave
        {
            LeaveId = 1,
            EmpId = 1,
            Status = LeaveStatus.Approved
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        // Act

        Func<Task> action =
            async () =>
                await _leaveService
                    .CancelLeaveAsync(1);

        // Assert

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage(
                "Only pending leaves can be cancelled");

        _leaveRepositoryMock.Verify(
            x => x.UpdateLeaveAsync(It.IsAny<Leave>()),
            Times.Never);
    }
}
