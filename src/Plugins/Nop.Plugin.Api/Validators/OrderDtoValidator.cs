using System.Collections.Generic;
using System.Net.Http;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    [UsedImplicitly]
    public class OrderDtoValidator : BaseDtoValidator<OrderDto>
    {
        #region Constructors

        public OrderDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) :
            base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetCustomerIdRule();
            SetOrderItemsRule();

            SetBillingAddressRule();
            SetShippingAddressRule();
        }

        #endregion

        #region Private Methods

        private void SetBillingAddressRule()
        {
            var key = "billing_address";
            if (RequestJsonDictionary.ContainsKey(key))
            {
                RuleFor(o => o.BillingAddress)
                    .SetValidator(new AddressDtoValidator(HttpContextAccessor, JsonHelper, (Dictionary<string, object>) RequestJsonDictionary[key]));
            }
        }

        private void SetShippingAddressRule()
        {
            var key = "shipping_address";
            if (RequestJsonDictionary.ContainsKey(key))
            {
                RuleFor(o => o.ShippingAddress)
                    .SetValidator(new AddressDtoValidator(HttpContextAccessor, JsonHelper, (Dictionary<string, object>) RequestJsonDictionary[key]));
            }
        }

        private void SetCustomerIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.CustomerId, "invalid customer_id", "customer_id");
        }

        private void SetOrderItemsRule()
        {
            var key = "order_items";
            if (RequestJsonDictionary.ContainsKey(key))
            {
                RuleForEach(c => c.OrderItems)
                    .Custom((orderItemDto, validationContext) =>
                    {
                        var orderItemJsonDictionary = GetRequestJsonDictionaryCollectionItemDictionary(key, orderItemDto);

                        var validator = new OrderItemDtoValidator(HttpContextAccessor, JsonHelper, orderItemJsonDictionary);

                        //force create validation for new addresses
                        if (orderItemDto.Id == 0)
                        {
                            validator.HttpMethod = HttpMethod.Post;
                        }

                        var validationResult = validator.Validate(orderItemDto);
                        MergeValidationResult(validationContext, validationResult);
                    });
            }
        }

        #endregion
    }
}
