using System.ComponentModel.DataAnnotations;

namespace _28_VuDucLuc_Assignment01_FrontEnd.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; } = string.Empty;
    }

    public class NewsArticleFormViewModel
    {
        public string NewsArticleId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline không được để trống")]
        public string Headline { get; set; } = string.Empty;

        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }
        public List<int> SelectedTagIds { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
    }

    public class AccountFormViewModel
    {
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string? AccountPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Role")]
        public int? AccountRole { get; set; }
    }

    public class CategoryFormViewModel
    {
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public List<Category> AllCategories { get; set; } = new();
    }

    public class ReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<NewsArticle> Articles { get; set; } = new();
    }
}
