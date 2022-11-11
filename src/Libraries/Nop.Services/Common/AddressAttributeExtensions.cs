using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class AddressAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this address attribute should have values
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                return false;

            if (addressAttribute.AttributeControlType == AttributeControlType.TextBox ||
                addressAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                addressAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                addressAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }
    }
}
