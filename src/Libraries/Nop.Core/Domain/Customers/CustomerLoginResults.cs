namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents the customer login result enumeration
    /// </summary>
    public enum CustomerLoginResults
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Customer does not exist (email or username)
        /// </summary>
        CustomerNotExist = 2,

        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword = 3,

        /// <summary>
        /// Account have not been activated
        /// </summary>
        NotActive = 4,

        /// <summary>
        /// Customer has been deleted 
        /// </summary>
        Deleted = 5,

        /// <summary>
        /// Customer not registered 
        /// </summary>
        NotRegistered = 6,

        /// <summary>
        /// Locked out
        /// </summary>
        LockedOut = 7,

        /// <summary>
        /// Requires multi-factor authentication
        /// </summary>
        MultiFactorAuthenticationRequired = 8
    }
}
