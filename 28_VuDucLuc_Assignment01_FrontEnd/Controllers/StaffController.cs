using _28_VuDucLuc_Assignment01_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _28_VuDucLuc_Assignment01_FrontEnd.Controllers
{
    public class StaffController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public StaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsStaff()
        {
            var role = HttpContext.Session.GetInt32("AccountRole");
            return role == 1 || role == 2;
        }

        private IActionResult RequireStaff() => RedirectToAction("Login", "Account");

        // ============================================================
        // DASHBOARD
        // ============================================================

        public IActionResult Index()
        {
            if (!IsStaff()) return RequireStaff();
            return View();
        }

        // ============================================================
        // PROFILE
        // ============================================================

        public async Task<IActionResult> Profile()
        {
            if (!IsStaff()) return RequireStaff();
            var id = HttpContext.Session.GetInt32("AccountId");
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.GetAsync($"odata/SystemAccounts/{id}");
            SystemAccount? account = null;
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                account = JsonSerializer.Deserialize<SystemAccount>(json, _jsonOptions);
            }
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] AccountFormViewModel model)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var payload = JsonSerializer.Serialize(new
            {
                accountId = model.AccountId,
                accountName = model.AccountName,
                accountEmail = model.AccountEmail,
                accountPassword = model.AccountPassword,
                accountRole = model.AccountRole
            });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"odata/SystemAccounts/{model.AccountId}", content);

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("AccountName", model.AccountName ?? "");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Cập nhật thất bại." });
        }

        // ============================================================
        // CATEGORIES
        // ============================================================

        public async Task<IActionResult> Categories()
        {
            if (!IsStaff()) return RequireStaff();
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.GetAsync("odata/Categories");
            List<Category> categories = new();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                categories = JsonSerializer.Deserialize<List<Category>>(json, _jsonOptions) ?? new();
            }
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryFormViewModel model)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var payload = JsonSerializer.Serialize(new
            {
                categoryId = model.CategoryId,
                categoryName = model.CategoryName,
                categoryDesciption = model.CategoryDesciption,
                parentCategoryId = model.ParentCategoryId,
                isActive = model.IsActive ?? true
            });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("odata/Categories", content);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false, message = "Tạo danh mục thất bại." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryFormViewModel model)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var payload = JsonSerializer.Serialize(new
            {
                categoryId = model.CategoryId,
                categoryName = model.CategoryName,
                categoryDesciption = model.CategoryDesciption,
                parentCategoryId = model.ParentCategoryId,
                isActive = model.IsActive ?? true
            });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"odata/Categories/{model.CategoryId}", content);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false, message = "Cập nhật danh mục thất bại." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.DeleteAsync($"odata/Categories/{id}");

            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            var err = await response.Content.ReadAsStringAsync();
            try
            {
                var errObj = JsonSerializer.Deserialize<Dictionary<string, string>>(err, _jsonOptions);
                return Json(new { success = false, message = errObj?["message"] ?? "Xóa thất bại." });
            }
            catch { return Json(new { success = false, message = "Xóa thất bại." }); }
        }

        // ============================================================
        // NEWS ARTICLES
        // ============================================================

        public async Task<IActionResult> NewsArticles()
        {
            if (!IsStaff()) return RequireStaff();
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.GetAsync("odata/NewsArticles");
            List<NewsArticle> articles = new();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                articles = JsonSerializer.Deserialize<List<NewsArticle>>(json, _jsonOptions) ?? new();
                articles = articles.OrderByDescending(a => a.CreatedDate).ToList();
            }
            return View(articles);
        }

        public async Task<IActionResult> MyArticles()
        {
            if (!IsStaff()) return RequireStaff();
            var id = HttpContext.Session.GetInt32("AccountId");
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.GetAsync($"odata/NewsArticles/byauthor/{id}");
            List<NewsArticle> articles = new();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                articles = JsonSerializer.Deserialize<List<NewsArticle>>(json, _jsonOptions) ?? new();
                articles = articles.OrderByDescending(a => a.CreatedDate).ToList();
            }
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> GetArticleFormData()
        {
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var catsTask = client.GetAsync("odata/Categories");
            var tagsTask = client.GetAsync("odata/Tags");
            await Task.WhenAll(catsTask, tagsTask);

            List<Category> cats = new();
            List<Tag> tags = new();

            if (catsTask.Result.IsSuccessStatusCode)
            {
                var json = await catsTask.Result.Content.ReadAsStringAsync();
                cats = JsonSerializer.Deserialize<List<Category>>(json, _jsonOptions) ?? new();
            }
            if (tagsTask.Result.IsSuccessStatusCode)
            {
                var json = await tagsTask.Result.Content.ReadAsStringAsync();
                tags = JsonSerializer.Deserialize<List<Tag>>(json, _jsonOptions) ?? new();
            }

            return Json(new { categories = cats, tags });
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] NewsArticleFormViewModel model)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var accountId = (short)(HttpContext.Session.GetInt32("AccountId") ?? 0);
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var payload = JsonSerializer.Serialize(new
            {
                newsArticleId = model.NewsArticleId,
                newsTitle = model.NewsTitle,
                headline = model.Headline,
                newsContent = model.NewsContent,
                newsSource = model.NewsSource,
                categoryId = model.CategoryId,
                newsStatus = model.NewsStatus ?? false,
                createdById = accountId,
                tagIds = model.SelectedTagIds
            });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("odata/NewsArticles", content);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = err });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateArticle([FromBody] NewsArticleFormViewModel model)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var accountId = (short)(HttpContext.Session.GetInt32("AccountId") ?? 0);
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var payload = JsonSerializer.Serialize(new
            {
                newsArticleId = model.NewsArticleId,
                newsTitle = model.NewsTitle,
                headline = model.Headline,
                newsContent = model.NewsContent,
                newsSource = model.NewsSource,
                categoryId = model.CategoryId,
                newsStatus = model.NewsStatus ?? false,
                updatedById = accountId,
                tagIds = model.SelectedTagIds
            });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"odata/NewsArticles/{model.NewsArticleId}", content);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            return Json(new { success = false, message = "Cập nhật thất bại." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            if (!IsStaff()) return Json(new { success = false, message = "Không có quyền." });

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.DeleteAsync($"odata/NewsArticles/{id}");

            return Json(response.IsSuccessStatusCode
                ? new { success = true }
                : new { success = false, message = "Xóa thất bại." });
        }
    }
}
