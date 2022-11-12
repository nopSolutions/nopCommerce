<<<<<<< HEAD
<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial record OnePageCheckoutModel : BaseNopModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisplayCaptcha { get; set; }
        public bool IsReCaptchaV3 { get; set; }
        public string ReCaptchaPublicKey { get; set; }

        public CheckoutBillingAddressModel BillingAddress { get; set; }
    }
=======
=======
=======
<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial record OnePageCheckoutModel : BaseNopModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisplayCaptcha { get; set; }
        public bool IsReCaptchaV3 { get; set; }
        public string ReCaptchaPublicKey { get; set; }

        public CheckoutBillingAddressModel BillingAddress { get; set; }
    }
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout
{
    public partial record OnePageCheckoutModel : BaseNopModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisplayCaptcha { get; set; }
        public bool IsReCaptchaV3 { get; set; }
        public string ReCaptchaPublicKey { get; set; }

        public CheckoutBillingAddressModel BillingAddress { get; set; }
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}