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
    public DateTime AssignedOn { get; set; }

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    public bool Status { get; set; } = true;

    [MaxLength(50)]
    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public Employee? Employee { get; set; }

    public Project? Project { get; set; }
}
