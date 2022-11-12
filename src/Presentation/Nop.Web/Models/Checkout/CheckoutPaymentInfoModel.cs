<<<<<<< HEAD
﻿using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial record CheckoutPaymentInfoModel : BaseNopModel
    {
        public Type PaymentViewComponent { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
=======
﻿using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial record CheckoutPaymentInfoModel : BaseNopModel
    {
        public Type PaymentViewComponent { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}