using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class VendorController : Controller
    {
        private readonly IRepository<Vendor> _vendorRepository;

        public VendorController(IRepository<Vendor> vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        [HttpGet]
        [Route("api/polycommerce/vendors")]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorRepository.Table
                .Where(x => !x.Deleted && x.Active)
                .Select(x => new PolyCommerceVendor { Name = x.Name,  VendorId = x.Id })
                .ToListAsync();

            return Ok(vendors);
        }
    }
}
