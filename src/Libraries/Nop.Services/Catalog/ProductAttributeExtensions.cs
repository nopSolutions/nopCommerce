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

            //other attribute control types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this product attribute can be used as condition for some other attribute
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>Result</returns>
        public static bool CanBeUsedAsCondition(this ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                return false;

            if (productAttributeMapping.AttributeControlType == AttributeControlType.ReadonlyCheckboxes || 
                productAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                productAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                productAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                productAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support it
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

            //other attribute control types does not have validation
            return false;
        }

        /// <summary>
        /// A value indicating whether this product attribute is non-combinable
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>Result</returns>
        public static bool IsNonCombinable(this ProductAttributeMapping productAttributeMapping)
        {
            //When you have a product with several attributes it may well be that some are combinable,
            //whose combination may form a new SKU with its own inventory,
            //and some non-combinable are more used to add accessories

            if (productAttributeMapping == null)
                return false;

            //we can add a new property to "ProductAttributeMapping" entity indicating whether it's combinable/non-combinable
            //but we assume that attributes
            //which cannot have values (any value can be entered by a customer)
            //are non-combinable
            var result = !ShouldHaveValues(productAttributeMapping);
            return result;
        }
    }
}
