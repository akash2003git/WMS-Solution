namespace WMS.Application.DTOs.Announcement;

public class CreateAnnouncementRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
