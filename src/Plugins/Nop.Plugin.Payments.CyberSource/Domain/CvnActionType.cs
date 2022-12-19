namespace Nop.Plugin.Payments.CyberSource.Domain
{
    /// <summary>
    /// Represents a CVN action type
    /// </summary>
    public enum CvnActionType
    {
        /// <summary>
        /// Ignore CVN results
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