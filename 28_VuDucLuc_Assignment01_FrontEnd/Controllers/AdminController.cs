using _28_VuDucLuc_Assignment01_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _28_VuDucLuc_Assignment01_FrontEnd.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsAdmin() => HttpContext.Session.GetInt32("AccountRole") == 0;

        private IActionResult RequireAdmin()
        {
            return RedirectToAction("Login", "Account");
        }

        // ============================================================
        // ACCOUNTS MANAGEMENT
        // ============================================================

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RequireAdmin();

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.GetAsync("odata/SystemAccounts");
            List<SystemAccount> accounts = new();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                accounts = JsonSerializer.Deserialize<List<SystemAccount>>(json, _jsonOptions) ?? new();
            }
            return View(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountFormViewModel model)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

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
            var response = await client.PostAsync("odata/SystemAccounts", content);

            if (response.IsSuccessStatusCode)
                return Json(new { success = true });

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = err });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountFormViewModel model)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

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
                return Json(new { success = true });

            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = err });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(short id)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.DeleteAsync($"odata/SystemAccounts/{id}");

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
        // REPORT
        // ============================================================

        public IActionResult Report()
        {
            if (!IsAdmin()) return RequireAdmin();
            return View(new ReportViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Report(ReportViewModel model)
        {
            if (!IsAdmin()) return RequireAdmin();

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var url = "odata/NewsArticles";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var all = JsonSerializer.Deserialize<List<NewsArticle>>(json, _jsonOptions) ?? new();

                // Filter theo ngày ở phía FrontEnd
                if (model.StartDate.HasValue)
                    all = all.Where(a => a.CreatedDate >= model.StartDate.Value).ToList();
                if (model.EndDate.HasValue)
                    all = all.Where(a => a.CreatedDate <= model.EndDate.Value.AddDays(1).AddSeconds(-1)).ToList();

                model.Articles = all.OrderByDescending(a => a.CreatedDate).ToList();
            }
            return View(model);
        }
    }
}
