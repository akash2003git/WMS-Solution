using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Leave
{
    [Key]
    public int LeaveId { get; set; }

    [ForeignKey(nameof(Employee))]
    public int EmpId { get; set; }

    [Required, MaxLength(30)]
    public LeaveType LeaveType { get; set; }

    [MaxLength(255)]
    public string? Reason { get; set; }

    [Required]
    public DateOnly FromDate { get; set; }

    [Required]
    public DateOnly ToDate { get; set; }

    [MaxLength(20)]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public Employee? Employee { get; set; }
}
