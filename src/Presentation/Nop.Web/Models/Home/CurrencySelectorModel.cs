using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Home;

namespace Nop.Web.Models.Home
{
    public class CurrencySelectorModel : BaseNopModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public CurrencyModel CurrentCurrency { get; set; }
    }
}