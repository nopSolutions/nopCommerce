using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the email account model factory implementation
    /// </summary>
    public partial class EmailAccountModelFactory : IEmailAccountModelFactory
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public EmailAccountModelFactory(EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService)
        {
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _localizationService = localizationService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare email account search model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>Email account search model</returns>
        public virtual EmailAccountSearchModel PrepareEmailAccountSearchModel(EmailAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged email account list model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>Email account list model</returns>
        public virtual EmailAccountListModel PrepareEmailAccountListModel(EmailAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get email accounts
            var emailAccounts = _emailAccountService.GetAllEmailAccounts().ToPagedList(searchModel);

            //prepare grid model
            var model = new EmailAccountListModel().PrepareToGrid(searchModel, emailAccounts, () =>
            {
                return emailAccounts.Select(emailAccount =>
                {
                    //fill in model values from the entity
                    var emailAccountModel = emailAccount.ToModel<EmailAccountModel>();

                    //fill in additional values (not existing in the entity)
                    emailAccountModel.IsDefaultEmailAccount = emailAccount.Id == _emailAccountSettings.DefaultEmailAccountId;

                    return emailAccountModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare email account model
        /// </summary>
        /// <param name="model">Email account model</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Email account model</returns>
        public virtual EmailAccountModel PrepareEmailAccountModel(EmailAccountModel model,
            EmailAccount emailAccount, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (emailAccount != null)
                model = model ?? emailAccount.ToModel<EmailAccountModel>();

            //set default values for the new model
            if (emailAccount == null)
                model.Port = 25;

            return model;
        }

        #endregion
    }
}