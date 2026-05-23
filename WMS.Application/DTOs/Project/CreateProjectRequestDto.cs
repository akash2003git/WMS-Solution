namespace WMS.Application.DTOs.Project;

public class CreateProjectRequestDto
{
    public string ProjectName { get; set; } = string.Empty;

    public int? ClientId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = "Active";
}
