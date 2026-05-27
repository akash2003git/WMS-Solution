using FluentAssertions;
using Moq;
using WMS.Application.Common.Exceptions;
using WMS.Application.DTOs.Employee;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Application.Interfaces;

namespace WMS.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository>
        _employeeRepositoryMock;
    private readonly Mock<ICurrentUserService>
        _currentUserServiceMock;

    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _employeeRepositoryMock =
            new Mock<IEmployeeRepository>();

        _currentUserServiceMock =
            new Mock<ICurrentUserService>();

        _employeeService =
            new EmployeeService(
                _employeeRepositoryMock.Object,
                _currentUserServiceMock.Object);

        _currentUserServiceMock
            .Setup(x => x.Role)
            .Returns("Admin");
    }

    [Fact]
    public async Task CreateEmployee_ValidRequest_CreatesEmployee()
    {
        // Arrange

        var request = new CreateEmployeeRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "9876543210",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = DateOnly.FromDateTime(DateTime.UtcNow),
            DepartmentId = 1,
            RoleId = 1
        };

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(request.Email))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(x => x.DepartmentExistsAsync(request.DepartmentId))
            .ReturnsAsync(true);

        _employeeRepositoryMock
            .Setup(x => x.RoleExistsAsync(request.RoleId))
            .ReturnsAsync(true);

        // Act

        var result =
            await _employeeService
                .CreateEmployeeAsync(request);

        // Assert

        result.Should().NotBeNull();

        result.Username.Should()
            .Be(request.Email);

        result.EmployeeId.Should()
            .BeGreaterThanOrEqualTo(0);

        result.TemporaryPassword.Should()
            .NotBeNullOrWhiteSpace();

        _employeeRepositoryMock.Verify(
            x => x.CreateEmployeeAsync(
                It.IsAny<Employee>(),
                It.IsAny<UserLogin>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_DuplicateEmail_ThrowsBusinessRuleException()
    {
        // Arrange

        var request = new CreateEmployeeRequestDto
        {
            Email = "existing@example.com",
            DepartmentId = 1,
            RoleId = 1
        };

        _employeeRepositoryMock
            .Setup(x => x.UsernameExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act

        Func<Task> action =
            async () =>
                await _employeeService
                    .CreateEmployeeAsync(request);

        // Assert

        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage("Email already exists");

        _employeeRepositoryMock.Verify(
            x => x.CreateEmployeeAsync(
                It.IsAny<Employee>(),
                It.IsAny<UserLogin>()),
            Times.Never);
    }

    [Fact]
    public async Task GetEmployeeById_ExistingEmployee_ReturnsEmployee()
    {
        // Arrange

        var employee = new Employee
        {
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "9999999999",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 1,
            RoleId = 1,

            Department = new Department
            {
                DepartmentName = "Engineering"
            },

            Role = new Role
            {
                RoleName = "Admin"
            }
        };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employee.EmployeeId))
            .ReturnsAsync(employee);

        // Act

        var result =
            await _employeeService
                .GetEmployeeByIdAsync(employee.EmployeeId);

        // Assert

        result.Should().NotBeNull();

        result.EmployeeId.Should()
            .Be(employee.EmployeeId);

        result.FullName.Should()
            .Be("John Doe");

        result.DepartmentName.Should()
            .Be("Engineering");

        result.RoleName.Should()
            .Be("Admin");
    }

    [Fact]
    public async Task GetEmployeeById_InvalidId_ThrowsNotFoundException()
    {
        // Arrange

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Employee?)null);

        // Act

        Func<Task> action =
            async () =>
                await _employeeService
                    .GetEmployeeByIdAsync(999);

        // Assert

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Employee not found");
    }

    [Fact]
    public async Task UpdateEmployee_ValidRequest_UpdatesEmployee()
    {
        // Arrange

        int employeeId = 1;

        var employee = new Employee
        {
            EmployeeId = employeeId,
            Email = "old@example.com"
        };

        var request = new UpdateEmployeeRequestDto
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com",
            PhoneNumber = "9999999999",
            Gender = 'F',
            DOB = new DateOnly(1999, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 1,
            RoleId = 1
        };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _employeeRepositoryMock
            .Setup(x => x.GetEmployeeByEmailAsync(request.Email))
            .ReturnsAsync((Employee?)null);

        _employeeRepositoryMock
            .Setup(x => x.DepartmentExistsAsync(request.DepartmentId))
            .ReturnsAsync(true);

        _employeeRepositoryMock
            .Setup(x => x.RoleExistsAsync(request.RoleId))
            .ReturnsAsync(true);

        // Act

        await _employeeService
            .UpdateEmployeeAsync(employeeId, request);

        // Assert

        employee.FirstName.Should()
            .Be("Updated");

        employee.Email.Should()
            .Be("updated@example.com");

        employee.Gender.Should()
            .Be('F');

        employee.UpdatedOn.Should()
            .NotBeNull();

        _employeeRepositoryMock.Verify(
            x => x.UpdateEmployeeAsync(employee),
            Times.Once);
    }

    [Fact]
    public async Task UpdateEmployee_ManagerFromDifferentDepartment_ThrowsForbiddenException()
    {
        // Arrange

        int employeeId = 1;

        var employee = new Employee
        {
            EmployeeId = employeeId,
            DepartmentId = 1,
            RoleId = 3,
            Email = "employee@test.com"
        };

        var request = new UpdateEmployeeRequestDto
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@test.com",
            PhoneNumber = "9999999999",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 1,
            RoleId = 3
        };

        _currentUserServiceMock
            .Setup(x => x.Role)
            .Returns("Manager");

        _currentUserServiceMock
            .Setup(x => x.DepartmentId)
            .Returns(2);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        // Act

        Func<Task> action =
            async () =>
                await _employeeService
                    .UpdateEmployeeAsync(employeeId, request);

        // Assert

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage(
                "You cannot modify employees outside your department");
    }

    [Fact]
    public async Task UpdateEmployee_ManagerChangesRole_ThrowsForbiddenException()
    {
        // Arrange

        int employeeId = 1;

        var employee = new Employee
        {
            EmployeeId = employeeId,
            DepartmentId = 2,
            RoleId = 3,
            Email = "employee@test.com"
        };

        var request = new UpdateEmployeeRequestDto
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "employee@test.com",
            PhoneNumber = "9999999999",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 2,
            RoleId = 2
        };

        _currentUserServiceMock
            .Setup(x => x.Role)
            .Returns("Manager");

        _currentUserServiceMock
            .Setup(x => x.DepartmentId)
            .Returns(2);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        // Act

        Func<Task> action =
            async () =>
                await _employeeService
                    .UpdateEmployeeAsync(employeeId, request);

        // Assert

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage(
                "Managers cannot change employee role");
    }

    [Fact]
    public async Task UpdateEmployee_ManagerChangesDepartment_ThrowsForbiddenException()
    {
        // Arrange

        int employeeId = 1;

        var employee = new Employee
        {
            EmployeeId = employeeId,
            DepartmentId = 2,
            RoleId = 3,
            Email = "employee@test.com"
        };

        var request = new UpdateEmployeeRequestDto
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "employee@test.com",
            PhoneNumber = "9999999999",
            Gender = 'M',
            DOB = new DateOnly(2000, 1, 1),
            DOJ = new DateOnly(2024, 1, 1),
            DepartmentId = 1,
            RoleId = 3
        };

        _currentUserServiceMock
            .Setup(x => x.Role)
            .Returns("Manager");

        _currentUserServiceMock
            .Setup(x => x.DepartmentId)
            .Returns(2);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        // Act

        Func<Task> action =
            async () =>
                await _employeeService
                    .UpdateEmployeeAsync(employeeId, request);

        // Assert

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage(
                "Managers cannot change employee department");
    }

}
