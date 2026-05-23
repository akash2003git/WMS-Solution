using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(80), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [RegularExpression("^[MFO]$")]
    public char Gender { get; set; }

    [Required]
    public DateOnly DOB { get; set; }

    [Required]
    public DateOnly DOJ { get; set; }

    [ForeignKey(nameof(Department))]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    [MaxLength(20)]
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedOn { get; set; }

    public Department? Department { get; set; }

    public Role? Role { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = [];

    public ICollection<Leave> Leaves { get; set; } = [];

    public ICollection<EmployeeProject> ProjectAllocations { get; set; } = [];

    public UserLogin? UserLogin { get; set; }
}
