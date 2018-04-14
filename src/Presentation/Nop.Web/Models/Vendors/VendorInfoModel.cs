using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(VendorInfoValidator))]
    public class VendorInfoModel : BaseNopModel
    {
        public VendorInfoModel()
        {
            this.VendorAttributes = new List<VendorAttributeModel>();
        }

        [NopResourceDisplayName("Account.VendorInfo.Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Account.VendorInfo.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }

        public IList<VendorAttributeModel> VendorAttributes { get; set; }
    }
}