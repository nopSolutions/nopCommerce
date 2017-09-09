using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Checkout;

namespace Nop.Web.Models.Checkout
{
    [Validator(typeof(CheckoutBillingAddressValidator))]
    public partial class CheckoutBillingAddressModel : BaseNopModel
    {
        public CheckoutBillingAddressModel()
        {
            ExistingAddresses = new List<AddressModel>();
            BillingNewAddress = new AddressModel();
        }

        //MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add to the model
        public IFormCollection Form { get; set; }

        public IList<AddressModel> ExistingAddresses { get; set; }

        public AddressModel BillingNewAddress { get; set; }


        public bool ShipToSameAddress { get; set; }
        public bool ShipToSameAddressAllowed { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool NewAddressPreselected { get; set; }
    }
}