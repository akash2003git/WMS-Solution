using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common.Responses;
using WMS.Application.DTOs.Department;
using WMS.Application.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(
        IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetDepartments()
    {
        var response =
            await _departmentService
                .GetDepartmentsAsync();

        return Ok(
            ApiResponse<List<DepartmentResponseDto>>
            .SuccessResponse(response));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetDepartmentById(int id)
    {
        var response =
            await _departmentService
                .GetDepartmentByIdAsync(id);

        return Ok(
            ApiResponse<DepartmentResponseDto>
            .SuccessResponse(response));
    }

    [HttpGet("{id}/employees")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult>
        GetDepartmentEmployees(int id)
    {
        var response =
            await _departmentService
                .GetDepartmentEmployeesAsync(id);

        return Ok(
            ApiResponse<List<DepartmentEmployeeDto>>
            .SuccessResponse(response));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        CreateDepartment(
            CreateDepartmentRequestDto request)
    {
        await _departmentService
            .CreateDepartmentAsync(request);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Department created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        UpdateDepartment(
            int id,
            UpdateDepartmentRequestDto request)
    {
        await _departmentService
            .UpdateDepartmentAsync(id, request);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Department updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        DeleteDepartment(int id)
    {
        await _departmentService
            .DeleteDepartmentAsync(id);

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                null!,
                "Department deleted successfully"));
    }
}
