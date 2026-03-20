using Microsoft.AspNetCore.Authorization;
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

        int? currentUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var claim = User.FindFirst("internal_user_id")?.Value;
            if (claim != null && int.TryParse(claim, out var uid))
                currentUserId = uid;
        }

        var vm = new AnnouncementIndexViewModel
        {
            Announcements = filtered.ToList(),
            CategoryFilter = category,
            SubCategoryFilter = subCategory,
            Categories = allCategories,
            CategoryOptions = configured,
            CurrentUserId = currentUserId
        };

        return View(vm);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoryOptionsAsync();
        return View(new CreateAnnouncementViewModel());
    }

    [Authorize]
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await apiService.GetByIdAsync(id);
        if (announcement == null) return NotFound();

        var currentUserId = GetCurrentUserId();
        if (currentUserId == null || announcement.UserId != currentUserId)
            return Forbid();

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

    [Authorize]
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

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await apiService.DeleteAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Announcement deleted successfully." : "Failed to delete announcement. Please try again.";
        return RedirectToAction(nameof(Index));
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst("internal_user_id")?.Value;
        return claim != null && int.TryParse(claim, out var uid) ? uid : null;
    }

    private async Task PopulateCategoryOptionsAsync()
    {
        var options = await apiService.GetCategoryOptionsAsync();
        ViewBag.CategoryOptions = options;
        ViewBag.Categories = options.Select(x => x.Name).OrderBy(x => x).ToList();
    }
}
