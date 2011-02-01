//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private ILocalizationService _localizationService;
        private Localizer _localizer;

        public override void InitHelpers()
        {
            base.InitHelpers();

            _localizationService = DependencyResolver.Current.GetService<ILocalizationService>();
        }

        public Localizer T
        {
            get
            {
                if (_localizer == null)
                {
                    //null localizer
                    //_localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));

                    //default localizer
                    _localizer = (format, args) =>
                    {
                        var resFormat = _localizationService.GetResource(format);
                        return new LocalizedString((args == null || args.Length == 0) ? resFormat : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {

    }
}
