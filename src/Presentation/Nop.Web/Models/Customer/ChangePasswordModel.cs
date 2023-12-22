using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer;

public partial record ChangePasswordModel : BaseNopModel
{
    [DataType(DataType.Password)]
    [NoTrim]
    [NopResourceDisplayName("Account.ChangePassword.Fields.OldPassword")]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    [NoTrim]
    [NopResourceDisplayName("Account.ChangePassword.Fields.NewPassword")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [NoTrim]
    [NopResourceDisplayName("Account.ChangePassword.Fields.ConfirmNewPassword")]
    public string ConfirmNewPassword { get; set; }
}