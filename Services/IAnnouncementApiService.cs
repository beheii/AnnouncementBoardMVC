using NoticeBoard_frontend.Models;

namespace NoticeBoard_frontend.Services;

public interface IAnnouncementApiService
{
    Task<List<AnnouncementViewModel>> GetAllAsync(string? category = null, string? subCategory = null);
    Task<List<CategoryOption>> GetCategoryOptionsAsync();
    Task<AnnouncementViewModel?> GetByIdAsync(int id);
    Task<AnnouncementViewModel?> CreateAsync(CreateAnnouncementViewModel model);
    Task<bool> UpdateAsync(int id, UpdateAnnouncementViewModel model);
    Task<bool> DeleteAsync(int id);
}
