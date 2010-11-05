using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration
{
    public partial class PromotionProviders : BaseNopAdministrationPage
    {
        protected override bool ValidatePageSecurity()
        {
            return IoCFactory.Resolve<IACLService>().IsActionAllowed("ManagePromotionProviders");
        } 
    }
}
