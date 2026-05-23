using System.ComponentModel.DataAnnotations;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class AuditLog
{
    [Key]
    public int AuditId { get; set; }

    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;

    public int RecordId { get; set; }

    [Required]
    [MaxLength(20)]
    public AuditAction Action { get; set; }

    public int? EmployeeId { get; set; }

    [MaxLength(100)]
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
        = DateTime.UtcNow;
}
