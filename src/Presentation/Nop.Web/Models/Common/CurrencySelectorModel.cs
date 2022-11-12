<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record CurrencySelectorModel : BaseNopModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public int CurrentCurrencyId { get; set; }
    }
=======
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record CurrencySelectorModel : BaseNopModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public int CurrentCurrencyId { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}