namespace Nop.Core.Domain.Gdpr
{
    /// <summary>
    /// Customer permanently deleted (GDPR)
    /// </summary>
    public class CustomerPermanenlyDeleted
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        public CustomerPermanenlyDeleted(int customerId, string email)
        {
            this.CustomerId = customerId;
            this.Email = email;
        }

        /// <summary>
        /// Customer identifier
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; }
    }
}