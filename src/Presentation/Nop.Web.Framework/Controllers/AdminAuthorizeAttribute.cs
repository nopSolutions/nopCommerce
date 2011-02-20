using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Controllers
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAdmin = false;
            //TODO inject IWorkContext
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var user = workContext.CurrentCustomer;
            if (user != null)
            {
                //TODO add some helper method (for example, ICustomerManager.IsAdmin(Customer customer))
                //TODO uncomment code below after we add authrorization pages (login/register)
                //isAdmin= user.CustomerRoles.Where(cr => cr.IsSystemRole && cr.SystemName == SystemCustomerRoleNames.Administrators).Any();
            }

            //remove code below after we add authrorization pages (login/register)
            isAdmin = true;
            return isAdmin;
        }
    }
}
