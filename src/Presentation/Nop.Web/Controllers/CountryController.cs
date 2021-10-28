using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        protected ICountryModelFactory CountryModelFactory { get; }
        
        #endregion
        
        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            CountryModelFactory = countryModelFactory;
		}
        
        #endregion
        
        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = await CountryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);
            
            return Json(model);
        }
        
        #endregion
    }
}