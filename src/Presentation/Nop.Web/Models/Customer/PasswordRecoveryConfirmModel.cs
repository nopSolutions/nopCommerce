using System.ComponentModel.DataAnnotations;
#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(PasswordRecoveryConfirmValidator))]
    public partial class PasswordRecoveryConfirmModel : BaseNopModel
    {
        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Account.PasswordRecovery.NewPassword")]
        public string NewPassword { get; set; }
        
        [NoTrim]
        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.PasswordRecovery.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public bool DisablePasswordChanging { get; set; }
        public string Result { get; set; }
    }
}