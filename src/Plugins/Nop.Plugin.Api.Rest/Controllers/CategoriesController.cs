using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET api/rest/categories?pageIndex=0&pageSize=20&name=
        [HttpGet]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetAll(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
            [FromQuery] string? name = null,
            [FromQuery] bool showHidden = false)
        {
            var index = PagingHelper.NormalizePageIndex(pageIndex);
            var size = PagingHelper.NormalizePageSize(pageSize);

            //the service requires the name to be non null, so an absent filter is passed as empty
            var categories = await _categoryService.GetAllCategoriesAsync(name ?? string.Empty,
                pageIndex: index, pageSize: size, showHidden: showHidden);

            return Ok(categories.ToPagedResult().Map(c => c.ToDto()));
        }

        // GET api/rest/categories/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return Ok(category.ToDto());
        }
    }
}
