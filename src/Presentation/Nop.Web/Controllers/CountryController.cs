#if NET451
using System.Web.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
#region Fields

        private readonly ICountryModelFactory _countryModelFactory;

#endregion

#region Constructors

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            this._countryModelFactory = countryModelFactory;
		}

#endregion

#region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetStatesByCountryId(countryId, addSelectStateItem);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

#endregion
    }
}
#endif