
namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Credit card duplicate check type enumeration. Indicates how (and whether) should check for duplicate cards. 
    /// </summary>
    public enum CardDuplicateCheckType
    {
        /// <summary>
        /// Does not check for duplicate card number for specified customer ID
        /// </summary>
        NoCheck = 0,

        /// <summary>
        /// Checks for duplicate card number for specified customer ID
        /// </summary>
        CheckWithinCustomer = 1,

        /// <summary>
        /// Checks for duplicate card number for all customer IDs for specified SecureNet ID 
        /// </summary>
        CheckWithinAllCustomers = 2,

        /// <summary>
        /// Checks for duplicate card number for all customer IDs for specified Group ID
        /// </summary>
        CheckWithinCustomersInGroup = 3
    }
}