using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcFrontend.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcFrontend
{
    class ShoppingCartItemHandler : IConsumer<EntityInsertedEvent<ShoppingCartItem>>
    {
        private readonly IRepository<ProductCartPrice> _productCartPriceRepository;
        private readonly IRepository<HiddenAttributeValue> _hiddenAttributeValueRepository;
        private readonly IProductService _productService;

        private readonly int addToCartForPriceAttrId;

        public ShoppingCartItemHandler(
            IRepository<ProductCartPrice> productCartPriceRepository,
            IRepository<HiddenAttributeValue> hiddenAttributeValueRepository,
            IRepository<HiddenAttribute> hiddenAttributeRepository,
            IProductService productService
        )
        {
            _productCartPriceRepository = productCartPriceRepository;
            _hiddenAttributeValueRepository = hiddenAttributeValueRepository;
            _productService = productService;

            var addToCartForPriceName = "Add To Cart For Price";

            var addToCartforPriceAttr = hiddenAttributeRepository.Table.Where(ha => ha.Name == addToCartForPriceName).FirstOrDefault();
            if (addToCartforPriceAttr == null)
            {
                addToCartforPriceAttr = new HiddenAttribute { Name = addToCartForPriceName };
                hiddenAttributeRepository.InsertAsync(addToCartforPriceAttr);
            }

            addToCartForPriceAttrId = addToCartforPriceAttr.Id;
        }

        public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            var sci = eventMessage.Entity;
            //if a cart price exists for the shopping cart item product we will add the difference as a hidden entity
            var prodCartPrice = _productCartPriceRepository.Table.Where(pcp => pcp.Product_Id == sci.ProductId).FirstOrDefault();
            if (prodCartPrice != null)
            {
                var product = await _productService.GetProductByIdAsync(sci.ProductId);
                var priceAdjustment = prodCartPrice.CartPrice - product.Price;

                // calculate from special price if it is being used instead of price
                var specialPriceEndDate = await product.GetSpecialPriceEndDateAsync();
                var specialPrice = await product.GetSpecialPriceAsync();
                if (specialPriceEndDate != null && specialPrice != 0.0M && specialPriceEndDate > DateTime.UtcNow)
                {
                    priceAdjustment = prodCartPrice.CartPrice - specialPrice;
                }

                await _hiddenAttributeValueRepository.InsertAsync(new HiddenAttributeValue
                {
                    ShoppingCartItem_Id = sci.Id,
                    HiddenAttribute_Id = addToCartForPriceAttrId,
                    PriceAdjustment = priceAdjustment
                });
            }
        }
    }
}
