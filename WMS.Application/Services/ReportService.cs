using WMS.Application.DTOs.Attendance;
using WMS.Application.DTOs.Report;
using WMS.Application.Interfaces;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProjectRepository _projectRepository;

    public ReportService(
        IAttendanceRepository attendanceRepository,
        ILeaveRepository leaveRepository,
        IEmployeeRepository employeeRepository,
        IProjectRepository projectRepository)
    {
        _attendanceRepository = attendanceRepository;
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _projectRepository = projectRepository;
    }

    public async Task<MonthlyAttendanceReportDto>
        GetMonthlyAttendanceReportAsync(
            int employeeId,
            int year,
            int month)
    {
        var fromDate = new DateOnly(year, month, 1);

        var toDate = fromDate.AddMonths(1).AddDays(-1);

        var records = await _attendanceRepository
            .GetAttendancesAsync(employeeId, fromDate, toDate);

        double totalHours = records
            .Where(a => a.TotalHours.HasValue)
            .Sum(a => a.TotalHours!.Value);

        var employee =
            await _employeeRepository.GetByIdAsync(employeeId);

        int daysInMonth =
            DateTime.DaysInMonth(year, month);

        int absentDays =
            daysInMonth - records.Count;

        double averageHours =
            records.Count == 0
                ? 0
                : totalHours / records.Count;

        return new MonthlyAttendanceReportDto
        {
            EmployeeId = employeeId,
            EmployeeName =
                $"{employee!.FirstName} {employee.LastName}",
            Year = year,
            Month = month,
            TotalPresentDays = records.Count,
            TotalAbsentDays = absentDays,
            TotalHoursWorked = totalHours,
            AverageHoursPerDay = averageHours,
            AttendancePercentage =
                daysInMonth == 0
                    ? 0
                    : (double)records.Count /
                      daysInMonth * 100
        };
    }

    public async Task<List<AttendanceResponseDto>>
        GetAttendanceHistoryAsync(int employeeId)
    {
        var list = await _attendanceRepository
            .GetAttendancesAsync(employeeId);

        return list.Select(a => new AttendanceResponseDto
        {
            AttendanceId = a.AttendanceId,
            EmployeeId = a.EmpId,
            EmployeeName =
                $"{a.Employee!.FirstName} {a.Employee.LastName}",
            AttendanceDate = a.AttendanceDate,
            CheckIn = a.CheckIn,
            CheckOut = a.CheckOut,
            TotalHours = a.TotalHours,
            WorkMode = a.WorkMode?.ToString() ?? string.Empty
        }).ToList();
    }

    public async Task<List<LeaveReportDto>>
        GetLeaveReportAsync(
            int? employeeId,
            LeaveStatus? status)
    {
        var leaves = await _leaveRepository
            .GetLeavesAsync(employeeId, status);

        return leaves.Select(l => new LeaveReportDto
        {
            LeaveId = l.LeaveId,
            EmployeeId = l.EmpId,
            EmployeeName =
                $"{l.Employee!.FirstName} {l.Employee.LastName}",
            LeaveType = l.LeaveType.ToString(),
            Status = l.Status.ToString(),
            FromDate = l.FromDate,
            ToDate = l.ToDate,
            AppliedOn = l.AppliedOn
        }).ToList();
    }

    public async Task<List<DepartmentEmployeeReportDto>>
        GetDepartmentEmployeeReportAsync(int departmentId)
    {
        var employees = await _employeeRepository
            .GetEmployeesAsync();

        return employees
            .Where(e => e.DepartmentId == departmentId)
            .Select(e => new DepartmentEmployeeReportDto
            {
                EmployeeId = e.EmployeeId,
                EmployeeName =
                    $"{e.FirstName} {e.LastName}",
                Email = e.Email,
                Role = e.Role!.RoleName,
                Status = e.Status.ToString()
            }).ToList();
    }

    public async Task<List<ProjectAllocationReportDto>>
        GetProjectAllocationReportAsync()
    {
        var projects = await _projectRepository
            .GetProjectsAsync();

        var report = new List<ProjectAllocationReportDto>();

        foreach (var project in projects)
        {
            var allocations =
                await _projectRepository
                    .GetProjectAllocationsAsync(project.ProjectId);

            report.AddRange(
                allocations.Select(a =>
                    new ProjectAllocationReportDto
                    {
                        ProjectId = project.ProjectId,
                        ProjectName = project.ProjectName,
                        EmployeeId = a.EmpId,
                        EmployeeName =
                            $"{a.Employee!.FirstName} {a.Employee.LastName}",
                        Department =
                            a.Employee.Department!.DepartmentName,
                        Role =
                            a.Employee.Role!.RoleName,
                        AssignedOn = a.AssignedOn
                    }));
        }

        return report;
    }
}
