using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class DiscountController : BaseNopController
    {
        #region Fields

        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Constructors

        public DiscountController(IDiscountService discountService,
            ILocalizationService localizationService, IWorkContext workContext)
        {
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        #endregion Constructors

        #region Discounts

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var discounts = _discountService.GetAllDiscounts(null, true);
            var gridModel = new GridModel<DiscountModel>
            {
                Data = discounts.Select(x => x.ToModel()),
                Total = discounts.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var discounts = _discountService.GetAllDiscounts(null, true);
            var gridModel = new GridModel<DiscountModel>
            {
                Data = discounts.Select(x => x.ToModel()),
                Total = discounts.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        //create
        public ActionResult Create()
        {
            var model = new DiscountModel();
            //default values
            model.LimitationTimes = 1;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(DiscountModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var discount = model.ToEntity();
                _discountService.InsertDiscount(discount);

                return continueEditing ? RedirectToAction("Edit", new { id = discount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            var discount = _discountService.GetDiscountById(id);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id", "id");
            var model = discount.ToModel();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(DiscountModel model, bool continueEditing)
        {
            var discount = _discountService.GetDiscountById(model.Id);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");
            if (ModelState.IsValid)
            {
                discount = model.ToEntity(discount);
                _discountService.UpdateDiscount(discount);

                return continueEditing ? RedirectToAction("Edit", discount.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var discount = _discountService.GetDiscountById(id);
            _discountService.DeleteDiscount(discount);
            return RedirectToAction("List");
        }

        #endregion
    }
}
