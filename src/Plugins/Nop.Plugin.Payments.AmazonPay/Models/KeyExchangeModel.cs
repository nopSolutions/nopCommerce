using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record KeyExchangeModel : BaseNopModel
{
    #region Properties

    public string Payload { get; set; }

    #endregion
}