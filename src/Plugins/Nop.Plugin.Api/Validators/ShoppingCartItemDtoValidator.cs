using System;
using System.Collections.Generic;
using System.Net.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTO.ShoppingCarts;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class ShoppingCartItemDtoValidator : BaseDtoValidator<ShoppingCartItemDto>
    {
        #region Constructors

        public ShoppingCartItemDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) :
            base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetCustomerIdRule();
            SetProductIdRule();
            SetQuantityRule();
            SetShoppingCartTypeRule();

            SetRentalDateRules();
        }

        #endregion

        #region Private Methods

        private void SetCustomerIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(x => x.CustomerId, "invalid customer_id", "customer_id");
        }

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(x => x.ProductId, "invalid product_id", "product_id");
        }

        private void SetQuantityRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(x => x.Quantity, "invalid quantity", "quantity");
        }

        private void SetRentalDateRules()
        {
            if (RequestJsonDictionary.ContainsKey("rental_start_date_utc") || RequestJsonDictionary.ContainsKey("rental_end_date_utc"))
            {
                RuleFor(x => x.RentalStartDateUtc)
                    .NotNull()
                    .WithMessage("Please provide a rental start date");

                RuleFor(x => x.RentalEndDateUtc)
                    .NotNull()
                    .WithMessage("Please provide a rental end date");

                RuleFor(dto => dto)
                    .Must(dto => dto.RentalStartDateUtc < dto.RentalEndDateUtc)
                    .WithMessage("Rental start date should be before rental end date");

                RuleFor(dto => dto)
                    .Must(dto => dto.RentalStartDateUtc > dto.CreatedOnUtc)
                    .WithMessage("Rental start date should be the future date");

                RuleFor(dto => dto)
                    .Must(dto => dto.RentalEndDateUtc > dto.CreatedOnUtc)
                    .WithMessage("Rental end date should be the future date");
            }
        }

        private void SetShoppingCartTypeRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("shopping_cart_type"))
            {
                RuleFor(x => x.ShoppingCartType)
                    .NotNull()
                    .Must(x => Enum.IsDefined(x))
                    .WithMessage("Please provide a valid shopping cart type");
            }
        }

        #endregion
    }
}
