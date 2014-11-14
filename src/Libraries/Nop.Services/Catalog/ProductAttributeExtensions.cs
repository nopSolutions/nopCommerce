
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this product attribute should have values
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                return false;

            if (productAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                productAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                productAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                productAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute controle types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this product attribute should can have some validation rules
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ValidationRulesAllowed(this ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                return false;

            if (productAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                productAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                productAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return true;

            //other attribute controle types does not have validation
            return false;
        }
    }
}
