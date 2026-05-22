using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities;

public class Attendance
{
    [Key]
    public int AttendanceId { get; set; }

    [ForeignKey(nameof(Employee))]
    public int EmpId { get; set; }

    [Required]
    public DateTime CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public double? TotalHours { get; set; }

    [MaxLength(20)]
    public string? WorkMode { get; set; } // WFO / WFH / Hybrid

    [Required]
    public DateTime AttendanceDate { get; set; }

    public Employee? Employee { get; set; }
}
