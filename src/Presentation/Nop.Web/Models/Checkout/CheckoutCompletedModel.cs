using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Checkout
{
    public class CheckoutCompletedModel : BaseNopModel
    {
        public int OrderId { get; set; }
    }
}