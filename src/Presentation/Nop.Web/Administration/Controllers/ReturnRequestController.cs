using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ReturnRequestController : BaseNopController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Constructors

        public ReturnRequestController(IOrderService orderService,
            ICustomerService customerService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, IWorkContext workContext)
        {
            this._orderService = orderService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        #endregion Constructors

        #region Utilities

        [NonAction]
        private void PrepareReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var opv = _orderService.GetOrderProductVariantById(returnRequest.OrderProductVariantId);
            model.Id = returnRequest.Id;
            model.ProductVariantId = opv.ProductVariantId;
            //product name
            if (!String.IsNullOrEmpty(opv.ProductVariant.Name))
                model.ProductName = string.Format("{0} ({1})", opv.ProductVariant.Product.Name, opv.ProductVariant.Name);
            else
                model.ProductName = opv.ProductVariant.Product.Name;
            model.OrderId = opv.OrderId;
            model.CustomerId = returnRequest.CustomerId;
            model.Quantity = returnRequest.Quantity;
            model.ReturnRequestStatusStr = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc).ToString();
            if (!excludeProperties)
            {
                model.ReasonForReturn = returnRequest.ReasonForReturn;
                model.RequestedAction = returnRequest.RequestedAction;
                model.CustomerComments = returnRequest.CustomerComments;
                model.StaffNotes = returnRequest.StaffNotes;
                model.ReturnRequestStatusId = returnRequest.ReturnRequestStatusId;
            }
        }

        #endregion

        #region Recurring payment

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var gridModel = new GridModel<ReturnRequestModel>();
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var returnRequests = _orderService.SearchReturnRequests(0, 0, null);
            var gridModel = new GridModel<ReturnRequestModel>
            {
                Data = returnRequests.PagedForCommand(command).Select(x =>
                {
                    var m = new ReturnRequestModel();
                    PrepareReturnRequestModel(m, x, false);
                    return m;
                }),
                Total = returnRequests.Count,
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //edit
        public ActionResult Edit(int id)
        {
            var returnRequest = _orderService.GetReturnRequestById(id);
            if (returnRequest == null)
                throw new ArgumentException("No return request found with the specified id", "id");
            
            var model = new ReturnRequestModel();
            PrepareReturnRequestModel(model, returnRequest, false);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(ReturnRequestModel model, bool continueEditing)
        {
            var returnRequest = _orderService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                throw new ArgumentException("No return request found with the specified id");

            if (ModelState.IsValid)
            {
                returnRequest.ReasonForReturn = model.ReasonForReturn;
                returnRequest.RequestedAction = model.RequestedAction;
                returnRequest.CustomerComments = model.CustomerComments;
                returnRequest.StaffNotes = model.StaffNotes;
                returnRequest.ReturnRequestStatusId = model.ReturnRequestStatusId;
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                _customerService.UpdateCustomer(returnRequest.Customer);

                return continueEditing ? RedirectToAction("Edit", returnRequest.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareReturnRequestModel(model, returnRequest, true);
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var returnRequest = _orderService.GetReturnRequestById(id);
            _orderService.DeleteReturnRequest(returnRequest);
            return RedirectToAction("List");
        }

        #endregion
    }
}
