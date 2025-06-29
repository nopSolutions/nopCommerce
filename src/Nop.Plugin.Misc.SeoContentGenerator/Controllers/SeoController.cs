using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.SeoContentGenerator.Services;

namespace Nop.Plugin.Misc.SeoContentGenerator.Controllers
{
    [Route("Admin/Seo")]
    public class SeoController : Controller
    {
        private readonly ISeoService _seoService;

        public SeoController(ISeoService seoService)
        {
            _seoService = seoService;
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> Generate(string input)
        {
            var result = await _seoService.GenerateSeoContentAsync(input);
            return Json(new { seoText = result });
        }
    }
}
