using _28_VuDucLuc_Assignment01_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _28_VuDucLuc_Assignment01_FrontEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AccountEmail") != null)
                return RedirectToHome();
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BackEndApi");
            var payload = JsonSerializer.Serialize(new { email = model.Email, password = model.Password });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Authentication/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<LoginResponse>(json, options);

            if (result == null)
            {
                ModelState.AddModelError("", "Đăng nhập thất bại.");
                return View(model);
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetString("AccountEmail", result.AccountEmail ?? "");
            HttpContext.Session.SetString("AccountName", result.AccountName ?? "");
            HttpContext.Session.SetInt32("AccountId", result.AccountId);
            HttpContext.Session.SetInt32("AccountRole", result.AccountRole);

            return RedirectToHome(result.AccountRole);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToHome(int role = -1)
        {
            return role == 0
                ? RedirectToAction("Index", "Admin")
                : role == 1 || role == 2
                    ? RedirectToAction("Index", "Staff")
                    : RedirectToAction("Index", "Home");
        }

        private class LoginResponse
        {
            public short AccountId { get; set; }
            public string? AccountName { get; set; }
            public string? AccountEmail { get; set; }
            public int AccountRole { get; set; }
        }
    }
}
