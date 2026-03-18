using System.Net.Http.Json;
using NoticeBoard_frontend.Models;

namespace NoticeBoard_frontend.Services;

public class AnnouncementApiService(HttpClient httpClient) : IAnnouncementApiService
{
    public async Task<List<AnnouncementViewModel>> GetAllAsync(string? category = null, string? subCategory = null)
    {
        var query = BuildQuery(category, subCategory);
        var result = await httpClient.GetFromJsonAsync<List<AnnouncementViewModel>>($"api/announcements{query}");
        return result ?? [];
    }

    public async Task<AnnouncementViewModel?> GetByIdAsync(int id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<AnnouncementViewModel>($"api/announcements/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<AnnouncementViewModel?> CreateAsync(CreateAnnouncementViewModel model)
    {
        var response = await httpClient.PostAsJsonAsync("api/announcements", model);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AnnouncementViewModel>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateAnnouncementViewModel model)
    {
        var response = await httpClient.PutAsJsonAsync($"api/announcements/{id}", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/announcements/{id}");
        return response.IsSuccessStatusCode;
    }

    private static string BuildQuery(string? category, string? subCategory)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(category))
            parts.Add($"category={Uri.EscapeDataString(category)}");
        if (!string.IsNullOrWhiteSpace(subCategory))
            parts.Add($"subCategory={Uri.EscapeDataString(subCategory)}");
        return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
    }
}
