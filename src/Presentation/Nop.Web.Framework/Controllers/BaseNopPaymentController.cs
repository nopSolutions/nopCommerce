using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Services.Payments;

namespace Nop.Web.Framework.Controllers
{
    public abstract class BaseNopPaymentController : Controller
    {
        public abstract IList<string> ValidatePaymentForm(FormCollection form);
        public abstract ProcessPaymentRequest GetPaymentInfo(FormCollection form);
    }
}
