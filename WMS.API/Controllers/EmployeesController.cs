using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Models;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetEmployees([FromQuery] EmployeeFilterDto filter)
    {
        var response = await _employeeService.GetEmployeesAsync(filter);

        return Ok(ApiResponse<PagedResponse<EmployeeResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var response = await _employeeService.GetEmployeeByIdAsync(id);

        return Ok(ApiResponse<EmployeeResponseDto>
            .SuccessResponse(response));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeRequestDto request)
    {
        var response = await _employeeService.CreateEmployeeAsync(request);

        return Ok(ApiResponse<CreateEmployeeResponseDto>
            .SuccessResponse(response, "Employee created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeRequestDto request)
    {
        await _employeeService.UpdateEmployeeAsync(id, request);

        return Ok(ApiResponse<string>
            .SuccessResponse(null!, "Employee updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        await _employeeService.DeleteEmployeeAsync(id);

        return Ok(ApiResponse<string>
            .SuccessResponse(null!, "Employee deleted successfully"));
    }
}
