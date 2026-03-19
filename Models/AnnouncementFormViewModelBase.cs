using System.ComponentModel.DataAnnotations;

namespace NoticeBoard_frontend.Models;

public abstract class AnnouncementFormViewModelBase
{
    public int? Id { get; set; }

    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Subcategory")]
    public string? SubCategory { get; set; }

    [Display(Name = "Status")]
    public bool Status { get; set; } = true;
}
