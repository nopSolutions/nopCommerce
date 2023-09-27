using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents a CyberSource customer token edit model
    /// </summary>
    public record CustomerTokenEditModel : BaseNopModel
    {
        #region Ctor

        public CustomerTokenEditModel()
        {
            CustomerToken = new CustomerTokenModel();
        }

        #endregion

        #region Properties

        public CustomerTokenModel CustomerToken { get; set; }

        #endregion
    }
}