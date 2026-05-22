using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Entities;

public class AuditLog
{
    [Key]
    public int AuditId { get; set; }

    public string? EntityName { get; set; }

    public int RecordId { get; set; }

    [MaxLength(20)]
    public string? Action { get; set; }   // Insert / Update / Delete

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
