using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Controllers
{
    public partial class ProductController
    {

        public virtual async Task<IActionResult> ViewCustomerMobileNumber(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToRoute("Homepage");

            var model = new ProductEmailAFriendModel();
            model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, false);

            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            var isPaidCustomer = await IsCustomerAPaidCustomer(currentCustomer);
            var allottedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(currentCustomer, NopCustomerDefaults.SubscriptionAllottedCount, storeId);
            var usedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(currentCustomer, NopCustomerDefaults.SubscriptionUsedCreditCount, storeId);

            if (!isPaidCustomer)
            {
                //customer not subscribed. Dispaly upgrade subscription View
                model.Result = "Please upgrade to premium Subscription to View contact details.Note: Previous unused credits will be carry forwarded if any..";
                return View(model);
            }

            //get user latest order
            var acitveOrder = (await _orderService.SearchOrdersAsync(customerId: currentCustomer.Id, psIds: new List<int> { (int)PaymentStatus.Paid })).FirstOrDefault();

            if (acitveOrder != null)
            {
                var allottedCreditCount1 = int.Parse(acitveOrder.CardType);
                var usedCreditCount1 = int.Parse(acitveOrder.CardType);
            }

            //check view count is valid ie under below allotted count from customer activity log
            var customerActivity = await _customerActivityService.GetAllActivitiesAsync(customerId: currentCustomer.Id, activityLogTypeId: 154, entityName: "Product");

            var props = (await _genericAttributeService.GetAttributesForEntityAsync(currentCustomer.Id, "Customer"))
                        .Where(x => x.StoreId == storeId && x.Key == "PublicStore.ViewContactDetail")
                        .ToList();

            var uniqueValues = props.GroupBy(x => x.Value).Select(grp => grp.First()).ToList();

            if (usedCreditCount >= allottedCreditCount)
            {
                //subscription limit reached
                model.Result = $"You have used {usedCreditCount} of {allottedCreditCount}. Please upgrade to premium.";
                return View(model);
            }

            //target customer
            var targetCustomer = await _customerService.GetCustomerByIdAsync(product.VendorId);
            var mobileNumber = targetCustomer.Phone;

            model.TargetCustomerMobileNumber = mobileNumber;
            model.TargetCustomerEmailId = targetCustomer.Email;

            if (!uniqueValues.Where(o => o.Value == targetCustomer.Id.ToString()).Any())
            {
                //update viewed credits
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.SubscriptionUsedCreditCount, (usedCreditCount + 1), storeId);

                var attribute = new Nop.Core.Domain.Common.GenericAttribute
                {
                    Key = "PublicStore.ViewContactDetail",
                    KeyGroup = "Customer",
                    Value = targetCustomer.Id.ToString(),
                    EntityId = currentCustomer.Id,
                    StoreId = storeId,
                    CreatedOrUpdatedDateUTC = DateTime.Now
                };

                //customer didnt view this customer contact detail previously. Insert in to generic attribute.
                await _genericAttributeService.InsertAttributeAsync(attribute);
                //await _genericAttributeService.SaveAttributeAsync(currentCustomer, "PublicStore.ViewContactDetail", targetCustomer.Id, storeId);

                //customer didnt view this customer contact detail previously.
                await _customerActivityService.InsertActivityAsync("PublicStore.ViewContactDetail", "Viewed Contact Details: First Time", product);
            }
            else
            {
                //customer is viewing the already viewed contact detail
                await _customerActivityService.InsertActivityAsync("PublicStore.ViewContactDetail", "Viewed Contact Details: Duplicate View", product);
            }

            return View(model);
        }

        private async Task<bool> IsCustomerAPaidCustomer(Customer customer)
        {
            var paidCustomer = false;
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            var subscriptionDate = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SubscriptionDate, storeId);

            //future implementation: check if subscription date is valid or not based on the subscription type 3, 6 or 1 year.

            paidCustomer = await _customerService.IsInCustomerRoleAsync(customer, "PaidCustomer");

            return paidCustomer;
        }


    }
}
