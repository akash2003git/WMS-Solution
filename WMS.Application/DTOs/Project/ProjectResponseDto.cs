namespace WMS.Application.DTOs.Project;

public class ProjectResponseDto
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public int? ClientId { get; set; }

    public string? ClientName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int TotalEmployees { get; set; }
}
