using System;
using System.Collections.Generic;
using System.Text;
using Nop.Services.Catalog;
using Nop.Services.Tasks;
using System.Linq;
using Nop.Services.Helpers;
using Nop.Services.Logging;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure
{

    public class WeekdayProductRotation : IScheduleTask
    {
        public const string PRODUCT_ROTATION_TASK = "Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure.WeekdayProductRotation";
        public const string BUSINESS_DAY_SPEC_OPTION_NAME = "Business Day";

        private readonly ISpecificationAttributeService _specificationAttribute;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public WeekdayProductRotation(ISpecificationAttributeService specificationAttribute,
            IScheduleTaskService scheduleTaskService, 
            IDateTimeHelper dateTimeHelper,
            IProductService productService,
            ILogger logger)
        {
            this._specificationAttribute = specificationAttribute;
            this._scheduleTaskService = scheduleTaskService;
            this._dateTimeHelper = dateTimeHelper;
            this._productService = productService;
            this._logger = logger;
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

        public void Execute()
        {
            var currentTask = _scheduleTaskService.GetTaskByType(PRODUCT_ROTATION_TASK);
            if (currentTask == null)
            {
                _logger.Warning("WeekdayProductRotation: Can't get current task info");
                throw new Exception("Can't get current task info");
            }

            var specificationAttributes = _specificationAttribute.GetSpecificationAttributes();
            var availableOnWeekdayAttribute = specificationAttributes.FirstOrDefault(x => x.Name.Equals("Available On Weekday"));
            if (availableOnWeekdayAttribute == null)
            {
                _logger.Warning("WeekdayProductRotation: Can't get available on weekday attribute");
                throw new Exception("Can't get available on weekday attribute");
            }
            
            var availableOnWeekdayOptions = 
                _specificationAttribute.GetSpecificationAttributeOptionsBySpecificationAttribute(availableOnWeekdayAttribute.Id);
            if (!availableOnWeekdayOptions.Any())
            {
                _logger.Warning("WeekdayProductRotation: Can't get options");
                throw new Exception("Can't get options");
            }

            var storeDateTime = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var today3PMLocal = new DateTime(storeDateTime.Year,
                storeDateTime.Month,
                storeDateTime.Day, 
                15, 
                0, 
                0);
            var today3PMUtc = _dateTimeHelper.ConvertToUtcTime(today3PMLocal);

            if(!currentTask.LastSuccessUtc.HasValue ||
                currentTask.LastSuccessUtc.Value <= today3PMUtc)
            {
                DayOfWeek targetWeekdayMenu;
                if (storeDateTime.Hour > 15)
                    targetWeekdayMenu = GetNextWeekDay(storeDateTime.DayOfWeek);
                else
                    targetWeekdayMenu = storeDateTime.DayOfWeek;

                var publishableSpecificationOptionIds = 
                    GetPublishableWeekdaySpecificationOptionIds(availableOnWeekdayOptions, targetWeekdayMenu);

                _logger.Information(
                    $"WeekdayProductRotation: Target weekday = {targetWeekdayMenu}, option ids = {string.Join(',', publishableSpecificationOptionIds)}");

                var allProducts = _productService.SearchProducts(showHidden: true);

                _logger.Information(
                    $"WeekdayProductRotation: Found {allProducts.TotalCount} products, " +
                    $"{allProducts.TotalPages} pages");

                int unpublishedCount = 0, publishedCount = 0, untouchedCount = 0;
                foreach (var product in allProducts)
                {
                    var productSpecificationAttributes = 
                        _specificationAttribute.GetProductSpecificationAttributes(productId: product.Id);
                    // publish / unpublish product only when it have at least one "Available On Weekday" product specification
                    if (productSpecificationAttributes.Any(x => 
                        availableOnWeekdayOptions.Any(y => y.Id == x.SpecificationAttributeOptionId)))
                    {
                        if (productSpecificationAttributes.Any(x =>
                            publishableSpecificationOptionIds.Contains(x.SpecificationAttributeOptionId)))
                        {
                            _logger.Information(
                                $"WeekdayProductRotation: Publishing product {product.Name}");
                            product.Published = true;
                            publishedCount++;
                        }
                        else
                        {
                            _logger.Information(
                                $"WeekdayProductRotation: Unpublishing product {product.Name}");
                            product.Published = false;
                            unpublishedCount++;
                        }

                        _productService.UpdateProduct(product);
                    }
                    else
                    {
                        _logger.Information(
                            $"WeekdayProductRotation: No weekday specification attribute for product {product.Name}");
                        untouchedCount++;
                    }
                }

                _logger.Information($"Published {publishedCount}, " +
                    $"unpublished {unpublishedCount}, " +
                    $"left untouched {untouchedCount} products");
            }
            else
            {
                _logger.Information("WeekdayProductRotation: Skipping for now");
            }
        }
    }
}
