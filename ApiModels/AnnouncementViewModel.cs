namespace NoticeBoard_frontend.ApiModels;

public class AnnouncementViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool Status { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
}
