using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Security;

namespace NopSolutions.NopCommerce.Web.Administration
{
    public partial class PaymentSettingsHome : BaseNopAdministrationPage
    {
        protected override bool ValidatePageSecurity()
        {
            return ACLManager.IsActionAllowed("ManagePaymentSettings");
        }
    }
}
