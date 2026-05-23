namespace WMS.Application.DTOs.Auth;

public class ResetPasswordDto
{
    public string Username { get; set; } = string.Empty;

    public string CurrentPassword { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}
