using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;

namespace Nop.Plugin.Api.Factories
{
    public class ProductFactory : IFactory<Product>
    {
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;

        public ProductFactory(IMeasureService measureService, MeasureSettings measureSettings)
        {
            _measureService = measureService;
            _measureSettings = measureSettings;
        }

        public Product Initialize()
        {
            var defaultProduct = new Product();
            
            defaultProduct.Weight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Ratio;

            defaultProduct.CreatedOnUtc = DateTime.UtcNow;
            defaultProduct.UpdatedOnUtc = DateTime.UtcNow;

            defaultProduct.ProductType = ProductType.SimpleProduct;

            defaultProduct.MaximumCustomerEnteredPrice = 1000;
            defaultProduct.MaxNumberOfDownloads = 10;
            defaultProduct.RecurringCycleLength = 100;
            defaultProduct.RecurringTotalCycles = 10;
            defaultProduct.RentalPriceLength = 1;
            defaultProduct.StockQuantity = 10000;
            defaultProduct.NotifyAdminForQuantityBelow = 1;
            defaultProduct.OrderMinimumQuantity = 1;
            defaultProduct.OrderMaximumQuantity = 10000;
            
            defaultProduct.UnlimitedDownloads = true;
            defaultProduct.IsShipEnabled = true;
            defaultProduct.AllowCustomerReviews = true;
            defaultProduct.Published = true;
            defaultProduct.VisibleIndividually = true;
           
            return defaultProduct;
        }
    }
}