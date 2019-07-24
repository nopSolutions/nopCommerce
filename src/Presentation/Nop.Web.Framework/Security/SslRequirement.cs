namespace Nop.Web.Framework.Security
{
    /// <summary>
    /// SSL requirement
    /// </summary>
    public enum SslRequirement
    {
        /// <summary>
        /// Page should be secured
        /// </summary>
        Yes,
        /// <summary>
        /// Page should not be secured
        /// </summary>
        No,
        /// <summary>
        /// It doesn't matter (as requested)
        /// </summary>
        NoMatter,
    }
}
