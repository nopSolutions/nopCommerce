using Nop.Core;

namespace Nop.Plugin.Payments.Synchrony.Domain
{
    public class ERPDetails : BaseEntity
    {
        /// Get or Set Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or Set Term Number
        /// </summary>
        public string TermNumber { get; set; }
    }
}
