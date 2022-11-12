<<<<<<< HEAD
<<<<<<< HEAD
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
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
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
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
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
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
