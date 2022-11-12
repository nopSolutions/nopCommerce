<<<<<<< HEAD
﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;
        
        #endregion
        
        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            _countryModelFactory = countryModelFactory;
		}
        
        #endregion
        
        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);
            
            return Json(model);
        }
        
        #endregion
    }
=======
﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;
        
        #endregion
        
        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            _countryModelFactory = countryModelFactory;
		}
        
        #endregion
        
        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);
            
            return Json(model);
        }
        
        #endregion
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}