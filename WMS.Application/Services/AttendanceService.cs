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

    public AttendanceService(IAttendanceRepository attendanceRepository, ICurrentUserService currentUser)
    {
        _attendanceRepository = attendanceRepository;
        _currentUser = currentUser;
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

        attendance.TotalHours = (attendance.CheckOut.Value - attendance.CheckIn).TotalHours;

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

    public async Task<MonthlyAttendanceReportDto> GetMonthlyReportAsync(int? employeeId = null)
    {
        int empId = employeeId ?? _currentUser.EmployeeId
          ?? throw new UnauthorizedException("Employee account required");

        var today = DateTime.UtcNow;

        DateOnly startDate = new(today.Year, today.Month, 1);

        DateOnly endDate = new(today.Year, today.Month,
                DateTime.DaysInMonth(today.Year, today.Month));

        var attendances = await _attendanceRepository
                .GetAttendancesAsync(empId, startDate, endDate);

        double totalHours = attendances.Sum(a => a.TotalHours ?? 0);

        int presentDays = attendances.Count;

        var employee = attendances.FirstOrDefault()?.Employee;

        DateOnly joiningDate = employee?.DOJ ?? startDate;

        DateOnly effectiveStartDate = joiningDate > startDate
                ? joiningDate
                : startDate;

        int totalWorkingDays = 0;

        for (DateOnly date = effectiveStartDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                totalWorkingDays++;
            }
        }

        return new MonthlyAttendanceReportDto
        {
            EmployeeId = empId,

            EmployeeName = attendances.FirstOrDefault()?.Employee != null
                    ? $"{attendances.First().Employee!.FirstName} {attendances.First().Employee!.LastName}"
                    : "",

            TotalPresentDays = presentDays,

            TotalAbsentDays = totalWorkingDays - presentDays,

            TotalHoursWorked = Math.Round(totalHours, 2),

            AverageHoursPerDay = presentDays == 0
                    ? 0
                    : Math.Round(totalHours / presentDays, 2)
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
