using _28_VuDucLuc_Assignment01_BackEnd.Models;
using _28_VuDucLuc_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _28_VuDucLuc_Assignment01_BackEnd.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
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
            var category = _repository.GetById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _repository.Add(category);
            return Created($"odata/Categories/{category.CategoryId}", category);
        }

        [HttpPut("{id}")]
        public IActionResult Put(short id, [FromBody] Category category)
        {
            if (id != category.CategoryId) return BadRequest("ID không khớp.");
            if (!_repository.Exists(id)) return NotFound();
            _repository.Update(category);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(short id)
        {
            if (!_repository.Exists(id)) return NotFound();
            bool deleted = _repository.Delete(id);
            if (!deleted)
                return Conflict(new { message = "Không thể xóa danh mục này vì đang được sử dụng trong bài viết." });
            return NoContent();
        }
    }
}
