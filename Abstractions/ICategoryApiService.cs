using NoticeBoard_frontend.ApiModels;

namespace NoticeBoard_frontend.Abstractions;

public interface ICategoryApiService
{
    Task<List<CategoryOption>> GetCategoryOptionsAsync();
}
