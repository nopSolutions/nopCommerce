using System.Linq;
using System.Web.Mvc;

using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.MVC.Areas.Admin.Models;

using Telerik.Web.Mvc;
using Nop.Core.Domain.Messages;
using System;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
	[AdminAuthorize]
    public class EmailAccountController : BaseNopController
	{
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountController(IEmailAccountService emailAccountService)
		{
			_emailAccountService = emailAccountService;
		}


		public ActionResult Index()
		{
			return View("List");
		}

		public ActionResult List()
		{

			var emailAccounts = _emailAccountService.GetAllEmailAccounts();
			var gridModel = new GridModel<EmailAccountModel>
			{
                Data = emailAccounts.Select(x => new EmailAccountModel(x)),
                Total = emailAccounts.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
		{
            var emailAccounts = _emailAccountService.GetAllEmailAccounts();
            var gridModel = new GridModel<EmailAccountModel>
			{
                Data = emailAccounts.Select(x => new EmailAccountModel(x)),
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
            var emailAccount = new EmailAccount();
            UpdateInstance(emailAccount, model);
            _emailAccountService.InsertEmailAccount(emailAccount);
            return RedirectToAction("Edit", new { id = emailAccount.Id });
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null) throw new ArgumentException("No email account found with the specified id", "id");
            return View(new EmailAccountModel(emailAccount));
        }

        [HttpPost]
        public ActionResult Edit(EmailAccountModel emailAccountModel)
        {
            if (!ModelState.IsValid)
            {
                return View(emailAccountModel);
            }
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountModel.Id);
            UpdateInstance(emailAccount, emailAccountModel);
            _emailAccountService.UpdateEmailAccount(emailAccount);
            return Edit(emailAccount.Id);
        }

        #endregion

        private static void UpdateInstance(EmailAccount instance, EmailAccountModel model)
        {
            instance.Id = model.Id;
            instance.Email = model.Email;
            instance.DisplayName = model.DisplayName;
            instance.Host = model.Host;
            instance.Port = model.Port;
            instance.Username = model.Username;
            instance.Password = model.Password;
            instance.EnableSsl = model.EnableSsl;
            instance.UseDefaultCredentials = model.UseDefaultCredentials;
        }
    }
}
