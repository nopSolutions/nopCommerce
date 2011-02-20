using System.Web.Mvc;
using Nop.Core.Infrastructure;
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

            _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
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
