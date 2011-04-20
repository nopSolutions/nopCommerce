using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class EmailAccountController : BaseNopController
	{
		private readonly IEmailAccountService _emailAccountService;
		private readonly EmailAccountSettings _emailAccountSettings;
		private readonly ISettingService _settingService;

		public EmailAccountController(IEmailAccountService emailAccountService
			, EmailAccountSettings emailAccountSettings, ISettingService settingService)
		{
			_emailAccountService = emailAccountService;
			_emailAccountSettings = emailAccountSettings;
			_settingService = settingService;
		}

		public ActionResult List(string id)
		{
			//mark as default email account (if selected)
			if (!String.IsNullOrEmpty(id))
			{
				int defaultEmailAccountId = Convert.ToInt32(id);
				var defaultEmailAccount = _emailAccountService.GetEmailAccountById(defaultEmailAccountId);
				if (defaultEmailAccount != null)
				{
					_emailAccountSettings.DefaultEmailAccountId = defaultEmailAccountId;
					_settingService.SaveSetting(_emailAccountSettings);
				}
			}

			var emailAccountModels = _emailAccountService.GetAllEmailAccounts()
									.Select(x => x.ToModel())
									.ToList();
			foreach (var eam in emailAccountModels)
				eam.IsDefaultEmailAccount = eam.Id == _emailAccountSettings.DefaultEmailAccountId;

			var gridModel = new GridModel<EmailAccountModel>
			{
				Data = emailAccountModels,
				Total = emailAccountModels.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
		{
            var emailAccountModels = _emailAccountService.GetAllEmailAccounts()
                                    .Select(x => x.ToModel())
                                    .ToList();
            foreach (var eam in emailAccountModels)
                eam.IsDefaultEmailAccount = eam.Id == _emailAccountSettings.DefaultEmailAccountId;

            var gridModel = new GridModel<EmailAccountModel>
            {
                Data = emailAccountModels,
                Total = emailAccountModels.Count()
            };

			return new JsonResult
			{
				Data = gridModel
			};
		}

		public ActionResult Create()
		{
			return View(new EmailAccountModel());
		}
        
        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
		public ActionResult Create(EmailAccountModel model, bool continueEditing)
		{
            if (ModelState.IsValid)
            {
                var emailAccount = model.ToEntity();
                _emailAccountService.InsertEmailAccount(emailAccount);
                return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

		public ActionResult Edit(int id)
		{
			var emailAccount = _emailAccountService.GetEmailAccountById(id);
			if (emailAccount == null) 
                throw new ArgumentException("No email account found with the specified id", "id");
			return View(emailAccount.ToModel());
		}
        
        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(EmailAccountModel model, bool continueEditing)
        {
            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                throw new ArgumentException("No email account found with the specified id", "id");
            if (ModelState.IsValid)
            {
                emailAccount = model.ToEntity(emailAccount);
                _emailAccountService.UpdateEmailAccount(emailAccount);
                return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                throw new ArgumentException("No email account found with the specified id", "id");

            _emailAccountService.DeleteEmailAccount(emailAccount);
            return RedirectToAction("List");
        }
	}
}
