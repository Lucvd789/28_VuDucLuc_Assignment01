using _28_VuDucLuc_Assignment01_BackEnd.Models;
using _28_VuDucLuc_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _28_VuDucLuc_Assignment01_BackEnd.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleRepository _repository;

        public NewsArticlesController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        // GET: odata/NewsArticles
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_repository.GetAll().AsQueryable());
        }

        // GET: odata/NewsArticles/active
        [HttpGet("active")]
        [EnableQuery]
        public IActionResult GetActive()
        {
            return Ok(_repository.GetActive().AsQueryable());
        }

        // GET: odata/NewsArticles/byauthor/{authorId}
        [HttpGet("byauthor/{authorId}")]
        [EnableQuery]
        public IActionResult GetByAuthor(short authorId)
        {
            return Ok(_repository.GetByAuthor(authorId).AsQueryable());
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public IActionResult Get(string id)
        {
            var article = _repository.GetById(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        [HttpPost]
        public IActionResult Post([FromBody] NewsArticleCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var article = new NewsArticle
            {
                NewsArticleId = request.NewsArticleId,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                CreatedById = request.CreatedById,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            _repository.Add(article, request.TagIds);
            return Created($"odata/NewsArticles/{article.NewsArticleId}", article);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] NewsArticleCreateRequest request)
        {
            if (id != request.NewsArticleId) return BadRequest("ID không khớp.");
            if (!_repository.Exists(id)) return NotFound();

            var article = new NewsArticle
            {
                NewsArticleId = request.NewsArticleId,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                UpdatedById = request.UpdatedById,
                ModifiedDate = DateTime.Now
            };

            _repository.Update(article, request.TagIds);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!_repository.Exists(id)) return NotFound();
            _repository.Delete(id);
            return NoContent();
        }
    }

    public class NewsArticleCreateRequest
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public short? UpdatedById { get; set; }
        public List<int>? TagIds { get; set; }
    }
}
