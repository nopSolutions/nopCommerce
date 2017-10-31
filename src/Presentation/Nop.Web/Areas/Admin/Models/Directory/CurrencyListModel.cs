using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    public partial class CurrencyListModel : BaseNopModel
    {
        public CurrencyListModel()
        {
            ExchangeRates = new List<ExchangeRateModel>();
            ExchangeRateProviders = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyRateAutoUpdateEnabled")]
        public bool AutoUpdateEnabled { get; set; }
        
        public IList<ExchangeRateModel> ExchangeRates { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.ExchangeRateProvider")]
        public string ExchangeRateProvider { get; set; }
        public IList<SelectListItem> ExchangeRateProviders { get; set; }

        #region Nested classes

        public class ExchangeRateModel : BaseNopModel
        {
            public string CurrencyCode { get; set; }

            public decimal Rate { get; set; }
        }

        #endregion
    }
}