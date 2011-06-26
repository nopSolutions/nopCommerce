using System;
using System.Web.Mvc;
using Nop.Plugin.Tax.StrikeIron.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.StrikeIron.Controllers
{
    [AdminAuthorize]
    public class TaxStrikeIronController : Controller
    {
        private readonly StrikeIronTaxSettings _strikeIronTaxSettings;
        private readonly ISettingService _settingService;

        public TaxStrikeIronController(StrikeIronTaxSettings strikeIronTaxSettings, ISettingService settingService)
        {
            this._strikeIronTaxSettings = strikeIronTaxSettings;
            this._settingService = settingService;
        }

        public ActionResult Configure()
        {
            var model = new TaxStrikeIronModel();
            model.UserId = _strikeIronTaxSettings.UserId;
            model.Password = _strikeIronTaxSettings.Password;
            model.TestingCanadaProvinceCode = "ON";
            model.TestingUsaZip = "10001";
            model.TestingUsaResult = "";
            model.TestingCanadaResult = "";
            return View("Nop.Plugin.Tax.StrikeIron.Views.TaxStrikeIron.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(TaxStrikeIronModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //clear testing results
            model.TestingUsaResult = "";
            model.TestingCanadaResult = "";

            //save settings
            _strikeIronTaxSettings.UserId = model.UserId;
            _strikeIronTaxSettings.Password = model.Password;
            _settingService.SaveSetting(_strikeIronTaxSettings);

            return View("Nop.Plugin.Tax.StrikeIron.Views.TaxStrikeIron.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("testUsa")]
        public ActionResult TestUsa(TaxStrikeIronModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //clear testing results
            model.TestingUsaResult = "";
            model.TestingCanadaResult = "";

            try
            {
                var strikeIronTaxProvider = new StrikeIronTaxProvider(_strikeIronTaxSettings);
                string zip = model.TestingUsaZip;
                string userId = model.UserId;
                string password = model.Password;
                string error = "";
                decimal taxRate = strikeIronTaxProvider.GetTaxRateUsa(zip, userId, password, ref error);
                if (!String.IsNullOrEmpty(error))
                    model.TestingUsaResult = error;
                else
                    model.TestingUsaResult = string.Format("Rate for zip {0}: {1}", zip, taxRate.ToString("p"));
            }
            catch (Exception exc)
            {
                model.TestingUsaResult = exc.ToString();
            }

            return View("Nop.Plugin.Tax.StrikeIron.Views.TaxStrikeIron.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("testCanada")]
        public ActionResult TestCanada(TaxStrikeIronModel model)
        {

            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //clear testing results
            model.TestingUsaResult = "";
            model.TestingCanadaResult = "";

            try
            {
                var strikeIronTaxProvider = new StrikeIronTaxProvider(_strikeIronTaxSettings);
                string province = model.TestingCanadaProvinceCode;
                string userId = model.UserId;
                string password = model.Password;
                string error = "";
                decimal taxRate = strikeIronTaxProvider.GetTaxRateCanada(province, userId, password, ref error);
                if (!String.IsNullOrEmpty(error))
                    model.TestingCanadaResult = error;
                else
                    model.TestingCanadaResult = string.Format("Rate for province {0}: {1}", province, taxRate.ToString("p"));
            }
            catch (Exception exc)
            {
                model.TestingCanadaResult = exc.ToString();
            }


            return View("Nop.Plugin.Tax.StrikeIron.Views.TaxStrikeIron.Configure", model);
        }

    }
}