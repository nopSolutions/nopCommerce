#region Using...

using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;

#endregion

namespace Nop.Web.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        #region Fields (2) 

        private ILocalizationService _localizationService;
        private Localizer _localizer;

        #endregion Fields 

        #region Properties (1) 

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
                                         if (string.IsNullOrEmpty(resFormat))
                                         {
                                             return new LocalizedString(format);
                                         }
                                         return
                                             new LocalizedString((args == null || args.Length == 0)
                                                                     ? resFormat
                                                                     : string.Format(resFormat, args));
                                     };
                }
                return _localizer;
            }
        }

        #endregion Properties 

        #region Methods (1) 

        public override void InitHelpers()
        {
            base.InitHelpers();

            _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        }

        #endregion Methods 
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}