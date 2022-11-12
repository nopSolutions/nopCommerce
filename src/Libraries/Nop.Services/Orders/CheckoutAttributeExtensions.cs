<<<<<<< HEAD
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

            //other attribute control types support values
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

            //other attribute control types support it
            return true;
        }
    }
=======
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

            //other attribute control types support values
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

            //other attribute control types support it
            return true;
        }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}