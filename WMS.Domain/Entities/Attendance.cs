using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Enums;

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

    public double? TotalHours { get; set; }

    [MaxLength(20)]
    public WorkMode? WorkMode { get; set; }

    [Required]
    public DateOnly AttendanceDate { get; set; }

    public Employee? Employee { get; set; }
}
