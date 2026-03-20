using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoticeBoard_frontend.Models;
using NoticeBoard_frontend.Services;

namespace NoticeBoard_frontend.Controllers;

public class AnnouncementsController(
    IAnnouncementApiService announcementApiService,
    ICategoryApiService categoryApiService,
    ILogger<AnnouncementsController> logger) : Controller
{
    [HttpGet]
    public IActionResult Error()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View();
    }

    public async Task<IActionResult> Index(string? category, string? subCategory)
    {
        var announcementsTask = announcementApiService.GetAllAsync(category, subCategory);
        var categoryOptionsTask = categoryApiService.GetCategoryOptionsAsync();
        await Task.WhenAll(announcementsTask, categoryOptionsTask);

        var vm = new AnnouncementIndexViewModel
        {
            Announcements = announcementsTask.Result,
            CategoryFilter = category,
            SubCategoryFilter = subCategory,
            CategoryOptions = categoryOptionsTask.Result,
            CurrentUserId = GetCurrentUserId()
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

        var result = await announcementApiService.CreateAsync(model);
        if (result == null)
        {
            logger.LogWarning("Create announcement failed for current user.");
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
        var announcement = await announcementApiService.GetByIdAsync(id);
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

        var success = await announcementApiService.UpdateAsync(id, model);
        if (!success)
        {
            logger.LogWarning("Update announcement failed for id {AnnouncementId}.", id);
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
        var announcement = await announcementApiService.GetByIdAsync(id);
        if (announcement == null)
        {
            TempData["ErrorMessage"] = "Announcement not found.";
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId == null || announcement.UserId != currentUserId)
        {
            return Forbid();
        }

        var success = await announcementApiService.DeleteAsync(id);
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
        var options = await categoryApiService.GetCategoryOptionsAsync();
        ViewBag.CategoryOptions = options;
        ViewBag.Categories = options.Select(x => x.Name).OrderBy(x => x).ToList();
    }
}
