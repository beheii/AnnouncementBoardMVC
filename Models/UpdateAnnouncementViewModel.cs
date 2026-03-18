using System.ComponentModel.DataAnnotations;

namespace NoticeBoard_frontend.Models;

public class UpdateAnnouncementViewModel
{
    public int Id { get; set; }

    [Display(Name = "Заголовок")]
    [Required(ErrorMessage = "Заголовок є обов'язковим")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Заголовок має містити від 5 до 100 символів")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Опис є обов'язковим")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "Опис має містити від 20 до 1000 символів")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Категорія")]
    [Required(ErrorMessage = "Категорія є обов'язковою")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Підкатегорія")]
    public string? SubCategory { get; set; }

    [Display(Name = "Статус")]
    public bool Status { get; set; }
}
