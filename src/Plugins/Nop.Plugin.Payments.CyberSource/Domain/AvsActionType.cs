namespace Nop.Plugin.Payments.CyberSource.Domain
{
    /// <summary>
    /// Represents an AVS action type
    /// </summary>
    public enum AvsActionType
    {
        /// <summary>
        /// Ignore AVS results
        /// </summary>
        Ignore = 0,

        /// <summary>
        /// Cancel Order
        /// </summary>
        Reject = 5,

        /// <summary>
        /// Verification Review
        /// </summary>
        Review = 10
    }
}