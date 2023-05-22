using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Checkout;
namespace Nop.Plugin.Payments.Iyzico.Models
{

 
    public record PaymentErrorModel : BaseNopModel
    {
        public PaymentErrorModel()
        {
            Warnings = new List<string>();
        }

        public IList<string> Warnings { get; set; }
    }
}