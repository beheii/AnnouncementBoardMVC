using Microsoft.AspNetCore.Mvc;
using NoticeBoard_frontend.Models;
using NoticeBoard_frontend.Services;

namespace NoticeBoard_frontend.Controllers;

public class AnnouncementsController(IAnnouncementApiService apiService, IConfiguration configuration) : Controller
{
    [HttpGet]
    public IActionResult Error()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View();
    }

    public async Task<IActionResult> Index(string? category, string? subCategory)
    {
        var all = await apiService.GetAllAsync();

        var filtered = all.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(category))
            filtered = filtered.Where(a => a.Category == category);
        if (!string.IsNullOrWhiteSpace(subCategory))
            filtered = filtered.Where(a => a.SubCategory == subCategory);

        var vm = new AnnouncementIndexViewModel
        {
            Announcements = filtered.ToList(),
            CategoryFilter = category,
            SubCategoryFilter = subCategory,
            Categories = all.Select(a => a.Category).Distinct().Order().ToList(),
            SubCategories = all
                .Where(a => a.SubCategory != null)
                .Select(a => a.SubCategory!)
                .Distinct()
                .Order()
                .ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoryOptionsAsync();
        return View(new CreateAnnouncementViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAnnouncementViewModel model)
    {
        await PopulateCategoryOptionsAsync();
        if (!ModelState.IsValid) return View(model);

        var result = await apiService.CreateAsync(model);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Не вдалося створити оголошення. Спробуйте ще раз.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Оголошення успішно створено.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await apiService.GetByIdAsync(id);
        if (announcement == null) return NotFound();

        await PopulateCategoryOptionsAsync();

        var vm = new UpdateAnnouncementViewModel
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Description = announcement.Description,
            Category = announcement.Category,
            SubCategory = announcement.SubCategory,
            Status = announcement.Status
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateAnnouncementViewModel model)
    {
        await PopulateCategoryOptionsAsync();
        if (!ModelState.IsValid) return View(model);

        var success = await apiService.UpdateAsync(id, model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Не вдалося оновити оголошення. Спробуйте ще раз.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Оголошення успішно оновлено.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await apiService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Оголошення успішно видалено.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoryOptionsAsync()
    {
        var configured = configuration.GetSection("CategoryOptions:Categories").Get<List<CategoryOption>>() ?? [];

        // Merge API-discovered values in case DB contains extra categories/subcategories
        var all = await apiService.GetAllAsync();
        var apiGroups = all
            .Where(a => !string.IsNullOrWhiteSpace(a.Category))
            .GroupBy(a => a.Category)
            .Select(g => new CategoryOption
            {
                Name = g.Key,
                SubCategories = g.Where(x => !string.IsNullOrWhiteSpace(x.SubCategory))
                    .Select(x => x.SubCategory!)
                    .Distinct()
                    .Order()
                    .ToList()
            })
            .ToList();

        var merged = configured
            .Concat(apiGroups)
            .GroupBy(c => c.Name)
            .Select(g => new CategoryOption
            {
                Name = g.Key,
                SubCategories = g.SelectMany(x => x.SubCategories).Distinct().Order().ToList()
            })
            .OrderBy(c => c.Name)
            .ToList();

        ViewBag.CategoryOptions = merged;
        ViewBag.Categories = merged.Select(x => x.Name).ToList();
    }
}
