using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [Route("api/polycommerce/categories")]
        public async Task<IActionResult> GetAllCategories(int page = 1, int pageSize = 100)
        {
            var storeToken = Request.Headers.TryGetValue("Store-Token", out var values) ? values.First() : null;

            var store = await PolyCommerceHelper.GetPolyCommerceStoreByToken(storeToken);

            if (store == null)
            {
                return Unauthorized();
            }

            var skipRecords = (page - 1) * pageSize;

            var categories = new List<PolyCommerceCategory>();
            int count = 0;

            categories = await _categoryRepository.Table
            .Skip(skipRecords)
            .Take(pageSize)
            .Select(x => new PolyCommerceCategory { CategoryId = x.Id, ParentCategoryId = x.ParentCategoryId, Name = x.Name })
            .ToListAsync();

            count = await _categoryRepository.Table.CountAsync();

            var response = new PolyCommerceApiResponse<PolyCommerceCategory>
            {
                Results = categories,
                TotalCount = count,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }
    }
}
