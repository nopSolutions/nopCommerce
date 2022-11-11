using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CustomerAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this customer attribute should have values
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                return false;

            if (customerAttribute.AttributeControlType == AttributeControlType.TextBox ||
                customerAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                customerAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                customerAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }
    }
}
