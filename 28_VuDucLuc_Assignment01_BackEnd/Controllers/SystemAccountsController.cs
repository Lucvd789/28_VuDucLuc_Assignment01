using _28_VuDucLuc_Assignment01_BackEnd.Models;
using _28_VuDucLuc_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _28_VuDucLuc_Assignment01_BackEnd.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountRepository _repository;

        public SystemAccountsController(ISystemAccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_repository.GetAll().AsQueryable());
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public IActionResult Get(short id)
        {
            var account = _repository.GetById(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _repository.Add(account);
            return Created($"odata/SystemAccounts/{account.AccountId}", account);
        }

        [HttpPut("{id}")]
        public IActionResult Put(short id, [FromBody] SystemAccount account)
        {
            if (id != account.AccountId) return BadRequest("ID không khớp.");
            if (!_repository.Exists(id)) return NotFound();
            _repository.Update(account);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(short id)
        {
            if (!_repository.Exists(id)) return NotFound();
            bool deleted = _repository.Delete(id);
            if (!deleted)
                return Conflict(new { message = "Không thể xóa tài khoản này vì đã có bài viết được tạo bởi tài khoản." });
            return NoContent();
        }
    }
}
