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
			var emailAccounts = _emailAccountService.GetAllEmailAccounts();
			var gridModel = new GridModel<EmailAccountModel>
			{
				Data = emailAccounts.Select(AutoMapper.Mapper.Map<EmailAccount, EmailAccountModel>),
				Total = emailAccounts.Count()
			};

			return new JsonResult
			{
				Data = gridModel
			};
		}

		#region Create

		public ActionResult Create()
		{
			return View(new EmailAccountModel());
		}

		[HttpPost]
		public ActionResult Create(EmailAccountModel model)
		{
			var emailAccount = model.ToEntity();
			_emailAccountService.InsertEmailAccount(emailAccount);
			return RedirectToAction("Edit", new { id = emailAccount.Id });
		}

		#endregion

		#region Edit

		public ActionResult Edit(int id)
		{
			var emailAccount = _emailAccountService.GetEmailAccountById(id);
			if (emailAccount == null) throw new ArgumentException("No email account found with the specified id", "id");
			return View(emailAccount.ToModel());
		}

		[HttpPost]
		public ActionResult Edit(EmailAccountModel emailAccountModel)
		{
			if (!ModelState.IsValid)
			{
				return View(emailAccountModel);
			}
			var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountModel.Id);
			emailAccount = emailAccountModel.ToEntity(emailAccount);
			_emailAccountService.UpdateEmailAccount(emailAccount);
			return Edit(emailAccount.Id);
		}

		#endregion
	}
}
