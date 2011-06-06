using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models;
using Nop.Admin.Models.Directory;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class CountryController : BaseNopController
	{
		#region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;

	    #endregion

		#region Constructors

        public CountryController(ICountryService countryService,
            IStateProvinceService stateProvinceService, ILocalizationService localizationService)
		{
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
		}

		#endregion Constructors 

        #region Countries

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var countries = _countryService.GetAllCountries(true);
            var model = new GridModel<CountryModel>
            {
                Data = countries.Select(x => x.ToModel()),
                Total = countries.Count
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CountryList(GridCommand command)
        {
            var countries = _countryService.GetAllCountries(true);
            var model = new GridModel<CountryModel>
            {
                Data = countries.Select(x => x.ToModel()),
                Total = countries.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }
        
        public ActionResult Create()
        {
            var model = new CountryModel();
            //default values
            model.Published = true;
            model.AllowsBilling = true;
            model.AllowsShipping = true;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CountryModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var country = model.ToEntity();
                _countryService.InsertCountry(country);
                return continueEditing ? RedirectToAction("Edit", new { id = country.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var country = _countryService.GetCountryById(id);
            if (country == null)
                throw new ArgumentException("No country found with the specified id", "id");
            return View(country.ToModel());
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CountryModel model, bool continueEditing)
        {
            var country = _countryService.GetCountryById(model.Id);
            if (country == null)
                throw new ArgumentException("No country found with the specified id");

            if (ModelState.IsValid)
            {
                country = model.ToEntity(country);
                _countryService.UpdateCountry(country);
                return continueEditing ? RedirectToAction("Edit", new { id = country.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var country = _countryService.GetCountryById(id);
            if (country == null)
                throw new ArgumentException("No country found with the specified id", "id");
            _countryService.DeleteCountry(country);
            return RedirectToAction("List");
        }


        #endregion

        #region States / provinces

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult States(int countryId, GridCommand command)
        {
            var states = _stateProvinceService.GetStateProvincesByCountryId(countryId, true)
                .Select(x => x.ToModel());

            var model = new GridModel<StateProvinceModel>
            {
                Data = states,
                Total = states.Count()
            };
            return new JsonResult
            {
                Data = model
            };
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult StateUpdate(int countryId, StateProvinceModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var state = _stateProvinceService.GetStateProvinceById(model.Id);
            state = model.ToEntity(state);
            _stateProvinceService.UpdateStateProvince(state);

            return States(model.CountryId, command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult StateAdd(int countryId, StateProvinceModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var state = new StateProvince { CountryId = countryId };
            state = model.ToEntity(state);
            _stateProvinceService.InsertStateProvince(state);

            return States(countryId, command);
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult StateDelete(int id, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var state = _stateProvinceService.GetStateProvinceById(id);
            int countryId = state.CountryId;
            _stateProvinceService.DeleteStateProvince(state);


            return States(countryId, command);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(string countryId, bool addEmptyStateIfRequired)
        {
            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
            var states = country != null ? _stateProvinceService.GetStateProvincesByCountryId(country.Id, true).ToList() : new List<StateProvince>();
            var result = (from s in states
                         select new { id = s.Id, name = s.Name }).ToList();
            if (addEmptyStateIfRequired && result.Count == 0)
                result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.OtherNonUS") });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
