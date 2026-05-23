using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        CreateEmployeeRequestDto request)
    {
        var response = await _employeeService
            .CreateEmployeeAsync(request);

        return Ok(
            ApiResponse<CreateEmployeeResponseDto>
            .SuccessResponse(
                response,
                "Employee created successfully"));
    }
}
