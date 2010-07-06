using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Web
{
    public partial class NewsLetterSubscriptionActivationPage : BaseNopPage
    {
        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }
    }
}
