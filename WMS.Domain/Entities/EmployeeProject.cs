using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities;

public class EmployeeProject
{
    [Key]
    public int AllocationId { get; set; }

    [ForeignKey(nameof(Employee))]
    public int EmpId { get; set; }

    [ForeignKey(nameof(Project))]
    public int ProjectId { get; set; }

    [Required]
    public DateOnly AssignedOn { get; set; }

    [Required]
    public DateOnly CreateDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required, MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public bool Status { get; set; } = true;

    [MaxLength(50)]
    public string? UpdatedBy { get; set; }

    public DateOnly? UpdatedDate { get; set; }

    public Employee? Employee { get; set; }

    public Project? Project { get; set; }
}
