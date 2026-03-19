using Microsoft.AspNetCore.Mvc;
using NoticeBoard_frontend.Models;
using NoticeBoard_frontend.Services;

namespace NoticeBoard_frontend.Controllers;

public class AnnouncementsController(IAnnouncementApiService apiService) : Controller
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
        var configured = await apiService.GetCategoryOptionsAsync();

        var filtered = all.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(category))
            filtered = filtered.Where(a => a.Category == category);
        if (!string.IsNullOrWhiteSpace(subCategory))
            filtered = filtered.Where(a => a.SubCategory == subCategory);

        var allCategories = configured.Select(c => c.Name)
            .Concat(all.Select(a => a.Category))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .Order()
            .ToList();

        var vm = new AnnouncementIndexViewModel
        {
            Announcements = filtered.ToList(),
            CategoryFilter = category,
            SubCategoryFilter = subCategory,
            Categories = allCategories,
            CategoryOptions = configured
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
            ModelState.AddModelError(string.Empty, "Failed to create announcement. Please try again.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Announcement created successfully.";
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
            ModelState.AddModelError(string.Empty, "Failed to update announcement. Please try again.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Announcement updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await apiService.DeleteAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Announcement deleted successfully." : "Failed to delete announcement. Please try again.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoryOptionsAsync()
    {
        var options = await apiService.GetCategoryOptionsAsync();
        ViewBag.CategoryOptions = options;
        ViewBag.Categories = options.Select(x => x.Name).OrderBy(x => x).ToList();
    }
}
