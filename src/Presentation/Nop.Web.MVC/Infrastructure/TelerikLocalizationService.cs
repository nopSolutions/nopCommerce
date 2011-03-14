using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Infrastructure;

namespace Nop.Web.MVC.Infrastructure
{
    public class TelerikLocalizationService : Telerik.Web.Mvc.Infrastructure.ILocalizationService
    {
        private string _resourceName;
        private Nop.Services.Localization.ILocalizationService _localizationService;
        private int _currentLanguageId;

        public TelerikLocalizationService(string resourceName, int currentLanguageId, Services.Localization.ILocalizationService localizationService)
        {
            _resourceName = resourceName;
            _currentLanguageId = currentLanguageId;
            _localizationService = localizationService;
        }

        public IDictionary<string, string> All()
        {
            return ScopedResources();
        }

        public bool IsDefault
        {
            get { return true; }
        }

        public string One(string key)
        {
            var resourceName = "Telerik." + _resourceName + "." + key;
            return _localizationService.GetResource(resourceName, _currentLanguageId, true, resourceName);
        }

        private IDictionary<string, string> ScopedResources()
        {
            var scope = "telerik." + _resourceName;
            return _localizationService.GetAllResourcesByLanguageId(_currentLanguageId).Where(x => x.Key.ToLower().StartsWith(scope)).ToDictionary(x => x.Key.Replace(scope,""),
                                                                                                    x => x.Value.ResourceValue);
        }
    }
}