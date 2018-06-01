namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Customer password changed event
    /// </summary>
    public class CustomerPasswordChangedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="password">Password</param>
        public CustomerPasswordChangedEvent(CustomerPassword password)
        {
            this.Password = password;
        }

        /// <summary>
        /// Customer password
        /// </summary>
        public CustomerPassword Password { get; }
    }
}