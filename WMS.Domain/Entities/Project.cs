using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Project
{
    [Key]
    public int ProjectId { get; set; }

    [Required, MaxLength(100)]
    public string ProjectName { get; set; } = string.Empty;

    [ForeignKey(nameof(Client))]
    public int? ClientId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [MaxLength(20)]
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public Client? Client { get; set; }
    public ICollection<EmployeeProject> EmployeeAllocations { get; set; } = [];
}
