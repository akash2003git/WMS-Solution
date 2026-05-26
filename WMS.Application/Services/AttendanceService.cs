using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Interfaces;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using WMS.Application.Common.Models;

namespace WMS.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveRepository _leaveRepository;

    public AttendanceService(
        IAttendanceRepository attendanceRepository,
        ICurrentUserService currentUser,
        IEmployeeRepository employeeRepository,
        ILeaveRepository leaveRepository)
    {
        _attendanceRepository = attendanceRepository;
        _currentUser = currentUser;
        _employeeRepository = employeeRepository;
        _leaveRepository = leaveRepository;
    }

    public async Task<AttendanceResponseDto> CheckInAsync(AttendanceRequestDto request)
    {
        int empId = _currentUser.EmployeeId ?? throw new UnauthorizedException("Employee account required");

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await _attendanceRepository.GetByEmployeeAndDateAsync(empId, today);

        if (existing != null)
        {
            throw new BusinessRuleException("Attendance already exists for today");
        }

        var attendance = new Attendance
        {
            EmpId = empId,
            AttendanceDate = today,
            CheckIn = DateTime.UtcNow,
            WorkMode = Enum.Parse<WorkMode>(request.WorkMode, true)
        };

        await _attendanceRepository.AddAttendanceAsync(attendance);

        return MapToDto(attendance);
    }

    public async Task<AttendanceResponseDto> CheckOutAsync()
    {
        int empId = _currentUser.EmployeeId ?? throw new UnauthorizedException("Employee account required");

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(empId, today);

        if (attendance == null)
        {
            throw new BusinessRuleException("Cannot check-out before check-in");
        }

        if (attendance.CheckOut.HasValue)
        {
            throw new BusinessRuleException("Already checked out today");
        }

        attendance.CheckOut = DateTime.UtcNow;

        attendance.TotalHours =
            Math.Round(
                (attendance.CheckOut.Value
                - attendance.CheckIn).TotalHours,
                2);

        await _attendanceRepository.UpdateAttendanceAsync(attendance);

        return MapToDto(attendance);
    }

    public async Task<TodayAttendanceDto>
        GetTodayAttendanceAsync()
    {
        if (!_currentUser.EmployeeId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Employee account not found");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var attendance =
            await _attendanceRepository
                .GetByEmployeeAndDateAsync(
                    _currentUser.EmployeeId.Value,
                    today);

        if (attendance is null)
        {
            return new TodayAttendanceDto
            {
                HasCheckedIn = false,
                HasCheckedOut = false
            };
        }

        return new TodayAttendanceDto
        {
            HasCheckedIn = true,
            HasCheckedOut = attendance.CheckOut.HasValue,
            CheckIn = attendance.CheckIn,
            CheckOut = attendance.CheckOut,
            TotalHours = attendance.TotalHours,
            WorkMode = attendance.WorkMode?.ToString()
        };
    }

    public async Task<PagedResponse<AttendanceResponseDto>>
        GetMyAttendanceAsync(
            AttendanceFilterDto filter)
    {
        int employeeId =
            _currentUser.EmployeeId
            ?? throw new UnauthorizedException(
                "Employee account required");

        var attendances =
            await _attendanceRepository
                .GetAttendancesAsync(
                    employeeId,
                    filter.FromDate,
                    filter.ToDate);

        IQueryable<Attendance> query =
            attendances.AsQueryable();

        query = query
            .OrderByDescending(a => a.AttendanceDate)
            .ThenByDescending(a => a.CheckIn);

        int totalCount = query.Count();

        var items = query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResponse<AttendanceResponseDto>
        {
            Items = items,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<MonthlyAttendanceReportDto>
        GetMonthlyReportAsync(
            int? employeeId = null)
    {
        int empId =
            employeeId
            ?? _currentUser.EmployeeId
            ?? throw new UnauthorizedException(
                "Employee account required");

        var employee =
            await _employeeRepository
                .GetByIdAsync(empId);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        var todayUtc = DateTime.UtcNow;

        DateOnly monthStart =
            new(todayUtc.Year, todayUtc.Month, 1);

        DateOnly today =
            DateOnly.FromDateTime(todayUtc);

        DateOnly effectiveStartDate =
            employee.DOJ > monthStart
                ? employee.DOJ
                : monthStart;

        var attendances =
            await _attendanceRepository
                .GetAttendancesAsync(
                    empId,
                    effectiveStartDate,
                    today);

        var approvedLeaves =
            await _leaveRepository
                .GetApprovedLeavesAsync(
                    empId,
                    effectiveStartDate,
                    today);

        DateOnly yesterday =
            today.AddDays(-1);

        int elapsedDays =
            yesterday.Day;

        int sundayCount = 0;

        for (
            DateOnly date = effectiveStartDate;
            date <= yesterday;
            date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                sundayCount++;
            }
        }

        int presentDays =
            attendances
                .Select(a => a.AttendanceDate)
                .Distinct()
                .Count();

        int leaveDays = 0;

        foreach (var leave in approvedLeaves)
        {
            DateOnly leaveStart =
                leave.FromDate > effectiveStartDate
                    ? leave.FromDate
                    : effectiveStartDate;

            DateOnly leaveEnd =
                leave.ToDate < yesterday
                    ? leave.ToDate
                    : yesterday;

            for (
                DateOnly date = leaveStart;
                date <= leaveEnd;
                date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Sunday)
                {
                    leaveDays++;
                }
            }
        }

        int expectedWorkingDays =
            elapsedDays
            - sundayCount
            - leaveDays;

        expectedWorkingDays =
            Math.Max(expectedWorkingDays, 0);

        int absentDays =
            Math.Max(
                expectedWorkingDays - presentDays,
                0);

        double attendancePercentage =
            expectedWorkingDays == 0
                ? 0
                : Math.Round(
                    (double)presentDays
                    / expectedWorkingDays * 100,
                    2);

        double totalHours =
            attendances.Sum(a =>
                a.TotalHours ?? 0);

        return new MonthlyAttendanceReportDto
        {
            EmployeeId = employee.EmployeeId,

            EmployeeName =
                $"{employee.FirstName} {employee.LastName}",

            Year = today.Year,

            Month = today.Month,

            TotalPresentDays = presentDays,

            TotalAbsentDays = absentDays,

            TotalHoursWorked =
                Math.Round(totalHours, 2),

            AverageHoursPerDay =
                presentDays == 0
                    ? 0
                    : Math.Round(
                        totalHours / presentDays,
                        2),

            AttendancePercentage =
                attendancePercentage
        };
    }

    public async Task<PagedResponse<AttendanceResponseDto>>
        GetAttendanceHistoryAsync(
            AttendanceFilterDto filter)
    {
        var attendances =
            await _attendanceRepository
                .GetAttendancesAsync(
                    filter.EmployeeId,
                    filter.FromDate,
                    filter.ToDate);

        IQueryable<Attendance> query =
            attendances.AsQueryable();

        query = query
            .OrderByDescending(a => a.AttendanceDate)
            .ThenByDescending(a => a.CheckIn);

        int totalCount = query.Count();

        var items = query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResponse<AttendanceResponseDto>
        {
            Items = items,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<AbsenteeDto>> GetAbsenteesAsync()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        var employees = await _attendanceRepository.GetAbsenteesAsync(today);

        return employees
            .Select(e => new AbsenteeDto
            {
                EmployeeId = e.EmployeeId,

                EmployeeName = $"{e.FirstName} {e.LastName}",

                Department = e.Department?.DepartmentName ?? "",

                Role = e.Role?.RoleName ?? ""
            })
            .ToList();
    }

    private static AttendanceResponseDto MapToDto(Attendance attendance)
    {
        return new AttendanceResponseDto
        {
            AttendanceId = attendance.AttendanceId,

            EmployeeId = attendance.EmpId,

            EmployeeName = $"{attendance.Employee?.FirstName} {attendance.Employee?.LastName}",

            AttendanceDate = attendance.AttendanceDate,

            CheckIn = attendance.CheckIn,

            CheckOut = attendance.CheckOut,

            TotalHours = attendance.TotalHours,

            WorkMode = attendance.WorkMode?.ToString()
        };
    }
}
