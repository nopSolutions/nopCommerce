using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Checkout;

namespace Nop.Web.Models.Checkout
{
    [Validator(typeof(CheckoutShippingAddressValidator))]
    public partial class CheckoutShippingAddressModel : BaseNopModel
    {
        public CheckoutShippingAddressModel()
        {
            Warnings = new List<string>();
            ExistingAddresses = new List<AddressModel>();
            NewAddress = new AddressModel();
            PickupPoints = new List<CheckoutPickupPointModel>();
        }

        //MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add to the model
        public IFormCollection Form { get; set; }

        public IList<string> Warnings { get; set; }

        public IList<AddressModel> ExistingAddresses { get; set; }
        public AddressModel NewAddress { get; set; }
        public bool NewAddressPreselected { get; set; }

        public IList<CheckoutPickupPointModel> PickupPoints { get; set; }
        public bool AllowPickUpInStore { get; set; }
        public bool PickUpInStore { get; set; }
        public bool PickUpInStoreOnly { get; set; }
        public bool DisplayPickupPointsOnMap { get; set; }
        public string GoogleMapsApiKey { get; set; }
    }
}