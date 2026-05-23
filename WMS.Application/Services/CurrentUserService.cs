using System.Security.Claims;
using WMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WMS.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId =>
        int.Parse(
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "0");

    public string Username =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.Name)?.Value
        ?? string.Empty;

    public string Role =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.Role)?.Value
        ?? string.Empty;

    public int? EmployeeId
    {
        get
        {
            var value =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst("EmployeeId")?.Value;

            return int.TryParse(value, out int employeeId)
                ? employeeId
                : null;
        }
    }
}
