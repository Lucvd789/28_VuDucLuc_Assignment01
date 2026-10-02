using _28_VuDucLuc_Assignment01_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _28_VuDucLuc_Assignment01_BackEnd.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            using var context = new FunewsManagementContext();
            return Ok(context.Tags.ToList().AsQueryable());
        }
    }
}
