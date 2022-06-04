using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Web.Models.PrivateMessages;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{

    public partial interface IPrivateMessagesModelFactory
    {

    }

    public partial class PrivateMessagesModelFactory
    {
        private async Task BuildCustomSendPrivateMessageModel(SendPrivateMessageModel model, Customer customerTo)
        {
            var _productService = EngineContext.Current.Resolve<IProductService>();
            var _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

            var toCustomer = await _customerService.GetCustomerByIdAsync(customerTo.Id);
            var toProduct = await _productService.GetProductByIdAsync(toCustomer.VendorId);

            model.CustomerToSeName = toProduct != null ? await _urlRecordService.GetSeNameAsync(toProduct) : string.Empty;

        }

        private async Task BuildCustomPrivateMessageModel(PrivateMessageModel model, PrivateMessage pm)
        {
            var _productService = EngineContext.Current.Resolve<IProductService>();
            var _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

            var fromCustomer = await _customerService.GetCustomerByIdAsync(pm.FromCustomerId);
            var toCustomer = await _customerService.GetCustomerByIdAsync(pm.ToCustomerId);

            var fromProduct = await _productService.GetProductByIdAsync(fromCustomer.VendorId);
            var toProduct = await _productService.GetProductByIdAsync(toCustomer.VendorId);

            model.CustomerFromSeName = fromProduct != null ? await _urlRecordService.GetSeNameAsync(fromProduct) : string.Empty;
            model.CustomerToSeName = toProduct != null ? await _urlRecordService.GetSeNameAsync(toProduct) : string.Empty;
            model.SenderSubject = pm.SenderSubject;
            model.IsSystemGenerated = pm.IsSystemGenerated;

            //chek if any one To Or from customer is a paid customer.
            var isFromCustomerPaid = await _customerService.IsInCustomerRoleAsync(fromCustomer, "PaidCustomer");
            var isToCustomerPaid = await _customerService.IsInCustomerRoleAsync(toCustomer, "PaidCustomer");

            if (isFromCustomerPaid || isToCustomerPaid)
                model.CanCustomerReply = true;
            else
                model.CanCustomerReply = false;

            //customization
            //sent items
            if (pm.FromCustomerId == (await _workContext.GetCurrentCustomerAsync()).Id && pm.IsSystemGenerated)
            {
                model.Subject = pm.SenderSubject;
                model.Message = pm.SenderBodyText;
            }
        }

    }
}
