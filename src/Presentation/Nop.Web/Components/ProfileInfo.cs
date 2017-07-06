using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class ProfileInfoViewComponent : ViewComponent
    {
        private readonly ICustomerService _customerService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileInfoViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
        {
            this._customerService = customerService;
            this._profileModelFactory = profileModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int customerProfileId)
        {
            var customer = _customerService.GetCustomerById(customerProfileId);
            if (customer == null)
                throw new ArgumentNullException("customer");

            var model = _profileModelFactory.PrepareProfileInfoModel(customer);
            return View(model);
        }
    }
}
