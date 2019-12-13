using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        public HomeController(IRepository<Customer> cRepository, IRepository<CustomerPassword> cpRepository)
        {
            var query = from c in cRepository.Table
                join cp in cpRepository.Table on c.Id equals cp.CustomerId
                where cp.PasswordSalt == "qXIzFdY="
                select c;

            var key = query.Expression.ToString();

            key.ToUpper();
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}