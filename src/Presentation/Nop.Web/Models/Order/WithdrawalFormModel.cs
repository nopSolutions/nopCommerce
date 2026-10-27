using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Order;

public partial record WithdrawalFormModel : BaseNopModel
{
    #region Properties

    [NopResourceDisplayName("ReturnRequests.Withdrawal.Fields.OrderNumber")]
    public string OrderNumber { get; set; }

    [NopResourceDisplayName("ReturnRequests.Withdrawal.Fields.EmailAddress")]
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    public bool DisplayCaptcha { get; set; }

    public string Result { get; set; }

    #endregion
}