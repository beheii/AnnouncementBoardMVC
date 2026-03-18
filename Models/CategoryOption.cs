namespace NoticeBoard_frontend.Models;

public class CategoryOption
{
    public string Name { get; set; } = string.Empty;
    public List<string> SubCategories { get; set; } = [];
}

