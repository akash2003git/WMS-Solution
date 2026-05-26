using FluentAssertions;
using Moq;
using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Interfaces;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class AttendanceServiceTests
{
    private readonly Mock<IAttendanceRepository>
        _attendanceRepositoryMock;

    private readonly Mock<ICurrentUserService>
        _currentUserMock;

    private readonly AttendanceService _attendanceService;

    public AttendanceServiceTests()
    {
        _attendanceRepositoryMock =
            new Mock<IAttendanceRepository>();

        _currentUserMock =
            new Mock<ICurrentUserService>();

        _attendanceService =
            new AttendanceService(
                _attendanceRepositoryMock.Object,
                _currentUserMock.Object);
    }

    [Fact]
    public async Task CheckIn_ValidRequest_CreatesAttendance()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var request = new AttendanceRequestDto
        {
            WorkMode = "WFO"
        };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetByEmployeeAndDateAsync(
                    1,
                    It.IsAny<DateOnly>()))
            .ReturnsAsync((Attendance?)null);

        // Act

        var result =
            await _attendanceService
                .CheckInAsync(request);

        // Assert

        result.Should().NotBeNull();

        result.EmployeeId.Should()
            .Be(1);

        result.WorkMode.Should()
            .Be("WFO");

        _attendanceRepositoryMock.Verify(
            x => x.AddAttendanceAsync(
                It.Is<Attendance>(a =>
                    a.EmpId == 1
                    &&
                    a.WorkMode == WorkMode.WFO)),
            Times.Once);
    }

    [Fact]
    public async Task CheckIn_DuplicateAttendance_ThrowsBusinessRuleException()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var request = new AttendanceRequestDto
        {
            WorkMode = "WFH"
        };

        var existingAttendance = new Attendance
        {
            AttendanceId = 1,
            EmpId = 1
        };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetByEmployeeAndDateAsync(
                    1,
                    It.IsAny<DateOnly>()))
            .ReturnsAsync(existingAttendance);

        // Act

        Func<Task> action =
            async () =>
                await _attendanceService
                    .CheckInAsync(request);

        // Assert

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage(
                "Attendance already exists for today");

        _attendanceRepositoryMock.Verify(
            x => x.AddAttendanceAsync(
                It.IsAny<Attendance>()),
            Times.Never);
    }

    [Fact]
    public async Task CheckOut_ValidAttendance_UpdatesCheckoutTime()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var attendance = new Attendance
        {
            AttendanceId = 1,
            EmpId = 1,
            CheckIn = DateTime.UtcNow.AddHours(-8),
            AttendanceDate =
                DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetByEmployeeAndDateAsync(
                    1,
                    It.IsAny<DateOnly>()))
            .ReturnsAsync(attendance);

        // Act

        var result =
            await _attendanceService
                .CheckOutAsync();

        // Assert

        result.Should().NotBeNull();

        attendance.CheckOut.Should()
            .NotBeNull();

        attendance.TotalHours.Should()
            .BeGreaterThan(0);

        _attendanceRepositoryMock.Verify(
            x => x.UpdateAttendanceAsync(attendance),
            Times.Once);
    }

    [Fact]
    public async Task CheckOut_WithoutCheckIn_ThrowsBusinessRuleException()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetByEmployeeAndDateAsync(
                    1,
                    It.IsAny<DateOnly>()))
            .ReturnsAsync((Attendance?)null);

        // Act

        Func<Task> action =
            async () =>
                await _attendanceService
                    .CheckOutAsync();

        // Assert

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage(
                "Cannot check-out before check-in");
    }

    [Fact]
    public async Task CheckOut_AlreadyCheckedOut_ThrowsBusinessRuleException()
    {
        // Arrange

        _currentUserMock
            .Setup(x => x.EmployeeId)
            .Returns(1);

        var attendance = new Attendance
        {
            AttendanceId = 1,
            EmpId = 1,
            CheckIn = DateTime.UtcNow.AddHours(-8),
            CheckOut = DateTime.UtcNow.AddMinutes(-5)
        };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetByEmployeeAndDateAsync(
                    1,
                    It.IsAny<DateOnly>()))
            .ReturnsAsync(attendance);

        // Act

        Func<Task> action =
            async () =>
                await _attendanceService
                    .CheckOutAsync();

        // Assert

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage(
                "Already checked out today");

        _attendanceRepositoryMock.Verify(
            x => x.UpdateAttendanceAsync(
                It.IsAny<Attendance>()),
            Times.Never);
    }
}
