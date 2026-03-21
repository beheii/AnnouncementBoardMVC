using System.Net.Http.Json;
using NoticeBoard_frontend.Abstractions;
using NoticeBoard_frontend.ApiModels;

namespace NoticeBoard_frontend.Services;

public class CategoryApiService(HttpClient httpClient, ILogger<CategoryApiService> logger) : ICategoryApiService
{
    public async Task<List<CategoryOption>> GetCategoryOptionsAsync()
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<CategoryOption>>("api/categories");
            return result ?? [];
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch category options from API.");
            return [];
        }
    }
}
