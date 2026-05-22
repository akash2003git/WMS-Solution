using System.ComponentModel.DataAnnotations;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class AuditLog
{
    [Key]
    public int AuditId { get; set; }

    public string? EntityName { get; set; }

    public int RecordId { get; set; }

    [MaxLength(20)]
    public AuditAction Action { get; set; }

    public int CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
