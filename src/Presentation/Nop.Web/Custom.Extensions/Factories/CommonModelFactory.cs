using System;

namespace Nop.Web.Factories
{

    public partial class CommonModelFactory
    {
        #region methods

        protected virtual bool IsShoppingCartEnabled()
        {
            var shoppingCartEnabled = false;
            var currentPageUrl = _webHelper.GetThisPageUrl(false);
           
            if (currentPageUrl.Contains("pricing", StringComparison.InvariantCultureIgnoreCase))
                shoppingCartEnabled = true;

            return shoppingCartEnabled;
        }

        #endregion
    }
}
