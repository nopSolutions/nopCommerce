namespace Nop.Core.Domain.Gdpr
{
    /// <summary>
    /// Represents a GDPR request type
    /// </summary>
    public enum GdprRequestType
    {
        /// <summary>
        /// Consent (agree)
        /// </summary>
        ConsentAgree = 1,

        /// <summary>
        /// Consent (disagree)
        /// </summary>
        ConsentDisagree = 5,

        /// <summary>
        /// Export data
        /// </summary>
        ExportData = 10,

        /// <summary>
        /// Delete customer
        /// </summary>
        DeleteCustomer = 15
    }
}
