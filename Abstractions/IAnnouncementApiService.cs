using NoticeBoard_frontend.ApiModels;
using NoticeBoard_frontend.ViewModels;

namespace NoticeBoard_frontend.Abstractions;

public interface IAnnouncementApiService
{
    Task<List<AnnouncementViewModel>> GetAllAsync(string? category = null, string? subCategory = null);
    Task<AnnouncementViewModel?> GetByIdAsync(int id);
    Task<AnnouncementViewModel?> CreateAsync(CreateAnnouncementViewModel model);
    Task<bool> UpdateAsync(int id, UpdateAnnouncementViewModel model);
    Task<bool> DeleteAsync(int id);
}
