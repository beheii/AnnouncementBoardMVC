using NoticeBoard_frontend.Models;

namespace NoticeBoard_frontend.Services;

public interface ICategoryApiService
{
    Task<List<CategoryOption>> GetCategoryOptionsAsync();
}
