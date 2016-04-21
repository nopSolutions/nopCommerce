using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CheckoutAttributeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether this checkout attribute should have values
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                return false;

            if (checkoutAttribute.AttributeControlType == AttributeControlType.TextBox ||
                checkoutAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                checkoutAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                checkoutAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;
            
            //other attribute controle types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this checkout attribute can be used as condition for some other attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Result</returns>
        public static bool CanBeUsedAsCondition(this CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                return false;

            if (checkoutAttribute.AttributeControlType == AttributeControlType.ReadonlyCheckboxes ||
                checkoutAttribute.AttributeControlType == AttributeControlType.TextBox ||
                checkoutAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                checkoutAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                checkoutAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute controle types support it
            return true;
        }

        /// <summary>
        /// Remove attributes which require shippable products
        /// </summary>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <returns>Result</returns>
        public static IList<CheckoutAttribute> RemoveShippableAttributes(this IList<CheckoutAttribute> checkoutAttributes)
        {
            if (checkoutAttributes == null)
                throw new ArgumentNullException("checkoutAttributes");

            return checkoutAttributes.Where(x => !x.ShippableProductRequired).ToList();
        }
    }
}
