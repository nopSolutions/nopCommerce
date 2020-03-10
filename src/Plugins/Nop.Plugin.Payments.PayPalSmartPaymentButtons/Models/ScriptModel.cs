using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models
{
    /// <summary>
    /// Represents a script model
    /// </summary>
    public class ScriptModel : BaseNopModel
    {
        #region Properties

        public string ScriptUrl { get; set; }

        #endregion
    }
}