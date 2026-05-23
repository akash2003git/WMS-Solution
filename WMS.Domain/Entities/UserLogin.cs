using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities;

public class UserLogin
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    [ForeignKey(nameof(Employee))]
    public int? EmployeeId { get; set; }

    public bool MustChangePassword { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    public Role Role { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
}
