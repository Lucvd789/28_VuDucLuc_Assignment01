using _28_VuDucLuc_Assignment01_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace _28_VuDucLuc_Assignment01_FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: / — Trang công khai, hiển thị bài viết Active
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BackEndApi");
            var response = await client.GetAsync("odata/NewsArticles/active");

            List<NewsArticle> articles = new();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                articles = JsonSerializer.Deserialize<List<NewsArticle>>(json, _jsonOptions) ?? new();
            }

            return View(articles);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
