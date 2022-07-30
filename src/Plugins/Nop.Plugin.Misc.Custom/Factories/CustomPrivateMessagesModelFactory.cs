using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Plugin.Misc.Custom.Factories
{
    public class CustomPrivateMessagesModelFactory : PrivateMessagesModelFactory
    {
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;


        public CustomPrivateMessagesModelFactory(CustomerSettings customerSettings,
            ForumSettings forumSettings, ICustomerService customerService,
            IDateTimeHelper dateTimeHelper, IForumService forumService,
            ILocalizationService localizationService, IStoreContext storeContext,
            IWorkContext workContext) : base(customerSettings, forumSettings,
                customerService, dateTimeHelper, forumService, localizationService,
                storeContext, workContext)
        {
            _workContext = workContext;
            _customerService = customerService;
            _customerSettings = customerSettings;
        }

        public override async Task<SendPrivateMessageModel> PrepareSendPrivateMessageModelAsync(Customer customerTo, PrivateMessage replyToPM)
        {
            if (customerTo == null)
                throw new ArgumentNullException(nameof(customerTo));

            var model = new SendPrivateMessageModel
            {
                ToCustomerId = customerTo.Id,
                CustomerToName = await _customerService.FormatUsernameAsync(customerTo),
                AllowViewingToProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customerTo)
            };

            if (replyToPM == null)
                return model;

            if (replyToPM.ToCustomerId == (await _workContext.GetCurrentCustomerAsync()).Id ||
                replyToPM.FromCustomerId == (await _workContext.GetCurrentCustomerAsync()).Id)
            {
                model.ReplyToMessageId = replyToPM.Id;
                model.Subject = $"Re: {replyToPM.Subject}";
                //model.SenderSubject = $"Re: {replyToPM.SenderSubject}";
            }

            //customization
            await BuildCustomSendPrivateMessageModel(model, customerTo);

            return model;
        }

        private async Task BuildCustomSendPrivateMessageModel(SendPrivateMessageModel model, Customer customerTo)
        {
            var _productService = EngineContext.Current.Resolve<IProductService>();
            var _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

            var toCustomer = await _customerService.GetCustomerByIdAsync(customerTo.Id);
            var toProduct = await _productService.GetProductByIdAsync(toCustomer.VendorId);

            model.CustomerToSeName = toProduct != null ? await _urlRecordService.GetSeNameAsync(toProduct) : string.Empty;

        }
    }
}
