using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Vendors
{
    public class VendorInfoModel : BaseNopModel
    {
        public VendorInfoModel()
        {
            VendorAttributes = new List<VendorAttributeModel>();
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