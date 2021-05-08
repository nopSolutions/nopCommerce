using System;
using System.Collections.Generic;
using System.Text;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using System.Threading.Tasks;
using Nop.Services.Configuration;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure
{

    public class WeekdayProductRotation : Services.Tasks.IScheduleTask
    {
        public const string PRODUCT_ROTATION_TASK = "Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure.WeekdayProductRotation";
        private static readonly string LAST_SUCCESSFUL_RUN_KEY = $"{PRODUCT_ROTATION_TASK}.LastSuccessfulRunUtc";

        public const string BUSINESS_DAY_SPEC_OPTION_NAME = "Business Day";

        public const string UNPUBLISH_PRODUCT_AFTER_SPEC_NAME = "Unpublish After Hour";

        private readonly ISpecificationAttributeService _specificationAttribute;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        public WeekdayProductRotation(ISpecificationAttributeService specificationAttribute,
            IDateTimeHelper dateTimeHelper,
            IProductService productService,
            ILogger logger,
            ISettingService settingService)
        {
            this._specificationAttribute = specificationAttribute;
            this._dateTimeHelper = dateTimeHelper;
            this._productService = productService;
            this._logger = logger;
            this._settingService = settingService;
        }

        DayOfWeek GetNextWeekDay(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
                return DayOfWeek.Monday;
            return dayOfWeek + 1;
        }

        IList<int> GetPublishableWeekdaySpecificationOptionIds(
            IList<Core.Domain.Catalog.SpecificationAttributeOption> availableOnWeekdayOptions,
            DayOfWeek targetWeekday)
        {
            var result = new List<int>();
            var targetWeekdayString = targetWeekday.ToString();

            result.Add(availableOnWeekdayOptions.First(x => x.Name.Equals(targetWeekdayString)).Id);

            if(targetWeekday != DayOfWeek.Saturday &&
                targetWeekday != DayOfWeek.Sunday)
            {
                result.Add(availableOnWeekdayOptions.First(x => x.Name.Equals(BUSINESS_DAY_SPEC_OPTION_NAME)).Id);
            }

            return result;
        }

        public async Task ExecuteAsync()
        {
            var specificationAttributes = await _specificationAttribute.GetSpecificationAttributesAsync();
            var availableOnWeekdayAttribute = specificationAttributes.FirstOrDefault(x => x.Name.Equals("Available On Weekday"));
            if (availableOnWeekdayAttribute == null)
            {
                await _logger.WarningAsync("WeekdayProductRotation: Can't get available on weekday attribute");
                throw new Exception("Can't get available on weekday attribute");
            }
            
            var availableOnWeekdayOptions = 
                await _specificationAttribute.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(availableOnWeekdayAttribute.Id);
            if (!availableOnWeekdayOptions.Any())
            {
                await _logger.WarningAsync("WeekdayProductRotation: Can't get options");
                throw new Exception("Can't get options");
            }

            var storeDateTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            var today3PMLocal = new DateTime(storeDateTime.Year,
                storeDateTime.Month,
                storeDateTime.Day, 
                15, 
                0, 
                0);
            var today3PMUtc = _dateTimeHelper.ConvertToUtcTime(today3PMLocal);

            //var lastSuccessfulRunUtc = 
            //    await _settingService.GetSettingByKeyAsync<DateTime>(
            //        LAST_SUCCESSFUL_RUN_KEY, 
            //        loadSharedValueIfNotFound: true);

            //if(lastSuccessfulRunUtc <= today3PMUtc)
            //{

            DayOfWeek targetWeekdayMenu;
            if (storeDateTime.Hour > 15)
                targetWeekdayMenu = GetNextWeekDay(storeDateTime.DayOfWeek);
            else
                targetWeekdayMenu = storeDateTime.DayOfWeek;

            var publishableSpecificationOptionIds = 
                GetPublishableWeekdaySpecificationOptionIds(availableOnWeekdayOptions, targetWeekdayMenu);

            await _logger.InformationAsync(
                $"WeekdayProductRotation: Target weekday = {targetWeekdayMenu}, option ids = {string.Join(',', publishableSpecificationOptionIds)}");

            var allProducts = await _productService.SearchProductsAsync(showHidden: true);

            await _logger.InformationAsync(
                $"WeekdayProductRotation: Found {allProducts.TotalCount} products, " +
                $"{allProducts.TotalPages} pages");

            int unpublishedCount = 0, publishedCount = 0, untouchedCount = 0;
            foreach (var product in allProducts)
            {
                var productSpecificationAttributes =
                    await _specificationAttribute.GetProductSpecificationAttributesAsync(productId: product.Id);
                // publish / unpublish product only when it have at least one "Available On Weekday" product specification
                if (productSpecificationAttributes.Any(x => 
                    availableOnWeekdayOptions.Any(y => y.Id == x.SpecificationAttributeOptionId)))
                {
                    if (productSpecificationAttributes.Any(x =>
                        publishableSpecificationOptionIds.Contains(x.SpecificationAttributeOptionId)))
                    {
                        await _logger.InformationAsync(
                            $"WeekdayProductRotation: Publishing product {product.Name}");
                        product.Published = true;
                        publishedCount++;
                    }
                    else
                    {
                        await _logger.InformationAsync(
                            $"WeekdayProductRotation: Unpublishing product {product.Name}");
                        product.Published = false;
                        unpublishedCount++;
                    }

                    await _productService.UpdateProductAsync(product);
                }
                else
                {
                    await _logger.InformationAsync(
                        $"WeekdayProductRotation: No weekday specification attribute for product {product.Name}");
                    untouchedCount++;
                }
            }

            await _settingService.SetSettingAsync<DateTime>(
                LAST_SUCCESSFUL_RUN_KEY,
                storeDateTime,
                clearCache: true);

            await _logger.InformationAsync($"Published {publishedCount}, " +
                $"unpublished {unpublishedCount}, " +
                $"left untouched {untouchedCount} products");

            //}
            //else
            //{
            //    await _logger.InformationAsync("WeekdayProductRotation: Skipping for now");
            //}
        }
    }
}
