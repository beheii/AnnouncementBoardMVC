using NoticeBoard_frontend.ApiModels;

namespace NoticeBoard_frontend.ViewModels;

public class AnnouncementIndexViewModel
{
    public IEnumerable<AnnouncementViewModel> Announcements { get; set; } = [];
    public string? CategoryFilter { get; set; }
    public string? SubCategoryFilter { get; set; }
    public List<CategoryOption> CategoryOptions { get; set; } = [];
    public int? CurrentUserId { get; set; }
}
