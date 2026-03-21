namespace NoticeBoard_frontend.ApiModels;

public class CategoryOption
{
    public string Name { get; set; } = string.Empty;
    public List<string> SubCategories { get; set; } = [];
}
