namespace NoticeBoard_frontend.Models;

public class AnnouncementIndexViewModel
{
    public IEnumerable<AnnouncementViewModel> Announcements { get; set; } = [];
    public string? CategoryFilter { get; set; }
    public string? SubCategoryFilter { get; set; }
    public List<string> Categories { get; set; } = [];
    public List<CategoryOption> CategoryOptions { get; set; } = [];
}
