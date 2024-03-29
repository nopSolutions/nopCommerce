﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutProgressModel : BaseNopModel
{
    public CheckoutProgressStep CheckoutProgressStep { get; set; }
}

public enum CheckoutProgressStep
{
    Cart,
    Address,
    Shipping,
    Payment,
    Confirm,
    Complete
}