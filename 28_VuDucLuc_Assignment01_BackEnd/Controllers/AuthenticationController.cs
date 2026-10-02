using _28_VuDucLuc_Assignment01_BackEnd.Models;
using _28_VuDucLuc_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _28_VuDucLuc_Assignment01_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ISystemAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ISystemAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Email và Password không được để trống." });

            // Ưu tiên kiểm tra tài khoản Admin từ appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (request.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase)
                && request.Password == adminPassword)
            {
                return Ok(new LoginResponse
                {
                    AccountId = 0,
                    AccountName = "Administrator",
                    AccountEmail = adminEmail!,
                    AccountRole = 0
                });
            }

            // Nếu không phải Admin, kiểm tra trong CSDL
            var account = _accountRepository.Login(request.Email, request.Password);
            if (account == null)
                return Unauthorized(new { message = "Email hoặc Password không đúng." });

            return Ok(new LoginResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName ?? "",
                AccountEmail = account.AccountEmail ?? "",
                AccountRole = account.AccountRole ?? -1
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int AccountRole { get; set; }
    }
}
