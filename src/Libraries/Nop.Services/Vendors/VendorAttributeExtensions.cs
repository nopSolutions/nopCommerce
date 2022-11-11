using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents vendor attribute extensions
    /// </summary>
    public static class VendorAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this vendor attribute should have values
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>True if the attribute should have values; otherwise false</returns>
        public static bool ShouldHaveValues(this VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                return false;

            if (vendorAttribute.AttributeControlType == AttributeControlType.TextBox ||
                vendorAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                vendorAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                vendorAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }
    }
}