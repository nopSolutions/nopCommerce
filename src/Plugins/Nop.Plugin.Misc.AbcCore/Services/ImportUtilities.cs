using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using System;
using System.Linq;
using SevenSpikes.Nop.Conditions.Domain;
using SevenSpikes.Nop.Conditions.Services;
using SevenSpikes.Nop.Framework.Domain.Enums;
using SevenSpikes.Nop.Mappings.Domain;
using SevenSpikes.Nop.Mappings.Domain.Enums;
using SevenSpikes.Nop.Mappings.Services;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using SevenSpikes.Nop.Plugins.NopQuickTabs.Domain;
using SevenSpikes.Nop.Plugins.NopQuickTabs.Services;
using SevenSpikes.Nop.Scheduling.Domain;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class ImportUtilities : IImportUtilities
    {
        private readonly IRepository<HtmlWidget> _htmlWidgetRepository;
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IRepository<Condition> _conditionRepository;
        private readonly IRepository<EntityCondition> _entityConditionRepository;
        private readonly IRepository<ConditionGroup> _conditionGroupRepository;
        private readonly IRepository<ConditionStatement> _conditionStatementRepository;
        private readonly IRepository<EntityWidgetMapping> _widgetMappingRepository;
        private readonly IRepository<ProductOverride> _productOverrideRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ITabService _tabService;
        private readonly IConditionService _conditionService;
        private readonly IEntityConditionService _entityConditionService;
        private readonly IEntityMappingService _entityMappingService;
        private readonly ILogger _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductAttributeService _productAttributeService;
        private readonly INopDataProvider _nopDbContext;

        public ImportUtilities(
            IRepository<HtmlWidget> htmlWidgetRepository,
            IRepository<Schedule> scheduleRepository,
            IRepository<Condition> conditionRepository,
            IRepository<EntityCondition> entityConditionRepository,
            IRepository<ConditionGroup> conditionGroupRepository,
            IRepository<ConditionStatement> conditionStatementRepository,
            IRepository<EntityWidgetMapping> widgetMappingRepository,
            IRepository<ProductOverride> productOverrideRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            ISpecificationAttributeService specificationAttributeService,
            ITabService tabService,
            IConditionService conditionService,
            IEntityConditionService entityConditionService,
            IEntityMappingService entityMappingService,
            ILogger logger,
            IRepository<Product> productRepository,
            IProductAttributeService productAttributeService,
            INopDataProvider nopDbContext)
        {
            _htmlWidgetRepository = htmlWidgetRepository;
            _scheduleRepository = scheduleRepository;
            _conditionRepository = conditionRepository;
            _entityConditionRepository = entityConditionRepository;
            _conditionGroupRepository = conditionGroupRepository;
            _conditionStatementRepository = conditionStatementRepository;
            _widgetMappingRepository = widgetMappingRepository;
            _productOverrideRepository = productOverrideRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _specificationAttributeService = specificationAttributeService;
            _tabService = tabService;
            _conditionService = conditionService;
            _entityConditionService = entityConditionService;
            _entityMappingService = entityMappingService;
            _logger = logger;
            _productRepository = productRepository;
            _productAttributeService = productAttributeService;
            _nopDbContext = nopDbContext;

            return;
        }

        #region HTML Widget Utilities

        /// <summary>
        ///		Create and insert an HTML widget
        ///		along with all the other needed entities.
        ///		It will have the provided name and HTML.
        ///		It will display in the provided widget zone
        ///		with the given display order.
        /// </summary>
        /// <returns>
        ///		The ID of the inserted HTML widget.
        /// </returns>
        public async Task<int> InsertHtmlWidgetAsync(
            string name, string html, string widgetZone, int displayOrder)
        {
            HtmlWidget widget = new HtmlWidget();
            widget.Name = name;
            widget.Visible = true;
            widget.HtmlContent = html;
            widget.LimitedToStores = false;
            await _htmlWidgetRepository.InsertAsync(widget);

            Schedule schedule = new Schedule();
            schedule.EntityType = EntityType.HtmlWidget;
            schedule.EntityId = widget.Id;
            schedule.EntityFromDate = null;
            schedule.EntityToDate = null;
            schedule.SchedulePatternType = SchedulePatternType.Everyday;
            schedule.SchedulePatternFromTime = TimeSpan.Zero;
            schedule.SchedulePatternToTime = TimeSpan.Zero;
            schedule.ExactDayValue = 1;
            schedule.EveryMonthFromDayValue = 1;
            schedule.EveryMonthToDayValue = 1;
            await _scheduleRepository.InsertAsync(schedule);

            Condition condition = new Condition();
            condition.Name = null;
            condition.Active = true;
            await _conditionRepository.InsertAsync(condition);

            EntityCondition entityCondition = new EntityCondition();
            entityCondition.ConditionId = condition.Id;
            entityCondition.EntityType = EntityType.HtmlWidget;
            entityCondition.EntityId = widget.Id;
            entityCondition.LimitedToStores = false;
            await _entityConditionRepository.InsertAsync(entityCondition);

            ConditionGroup conditionGroup = new ConditionGroup();
            conditionGroup.ConditionId = condition.Id;
            await _conditionGroupRepository.InsertAsync(conditionGroup);

            ConditionStatement conditionStatement = new ConditionStatement();
            conditionStatement.ConditionType = ConditionType.Default;
            conditionStatement.ConditionProperty = 0;
            conditionStatement.OperatorType = OperatorType.EqualTo;
            conditionStatement.Value = "Fail";
            conditionStatement.ConditionGroupId = conditionGroup.Id;
            await _conditionStatementRepository.InsertAsync(conditionStatement);

            EntityWidgetMapping widgetMapping = new EntityWidgetMapping();
            widgetMapping.EntityType = EntityType.HtmlWidget;
            widgetMapping.EntityId = widget.Id;
            widgetMapping.WidgetZone = widgetZone;
            widgetMapping.DisplayOrder = displayOrder;
            await _widgetMappingRepository.InsertAsync(widgetMapping);

            return widget.Id;
        }

        /// <summary>
        ///		Set the end date for when the widget will appear.
        /// </summary>
        /// <param name="widgetId">
        ///		The ID of the widget that is to be given an end date.
        /// </param>
        /// <param name="endDate">
        ///		The date when the widget is to no longer appear.
        ///	</param>
        public async Task SetWidgetEndDateAsync(int widgetId, DateTime endDate)
        {
            Schedule schedule = _scheduleRepository.Table.Where
                (
                    s => (s.EntityType == EntityType.HtmlWidget) &&
                        (s.EntityId == widgetId)
                )
                .FirstOrDefault();
            schedule.EntityToDate = endDate;
            await _scheduleRepository.UpdateAsync(schedule);

            return;
        }

        #endregion

        #region Quick Tabs Utilities

        /// <summary>
        ///		Create and insert a quick tab
        ///		along with all the other needed entities.
        ///		It will have the provided system name, display name, and HTML.
        ///		It will display ONLY for a single product whose ID is given.
        /// </summary>
        public async Task InsertQuickTabForSpecificProductAsync(
            string systemName, string displayName, string html, int productId)
        {
            Tab tab = new Tab();
            tab.SystemName = systemName;
            tab.DisplayName = displayName;
            tab.Description = html;
            tab.LimitedToStores = false;
            tab.TabMode = TabMode.Mappings;
            tab.DisplayOrder = 0;
            await _tabService.InsertTabAsync(tab);

            Condition condition = await _conditionService.CreateConditionAsync();
            condition.Active = true;
            await _conditionService.UpdateConditionAsync(condition);

            EntityCondition entityCondition = new EntityCondition();
            entityCondition.ConditionId = condition.Id;
            entityCondition.EntityType = EntityType.Tab;
            entityCondition.EntityId = tab.Id;
            entityCondition.LimitedToStores = false;
            await _entityConditionService.InsertEntityConditionAsync(entityCondition);

            ConditionGroup conditionGroup = new ConditionGroup();
            await _conditionService.CreateConditionGroupAsync(condition, conditionGroup);

            ConditionStatement conditionStatement = new ConditionStatement();
            await _conditionService.CreateConditionStatementAsync(conditionGroup, conditionStatement);
            conditionStatement.Value = "Fail";
            await _conditionService.UpdateConditionStatementAsync(conditionStatement);

            EntityMapping entityMapping = new EntityMapping();
            entityMapping.EntityType = EntityType.Tab;
            entityMapping.EntityId = tab.Id;
            entityMapping.MappedEntityId = productId;
            entityMapping.DisplayOrder = 0;
            entityMapping.MappingType = MappingType.Product;
            await _entityMappingService.InsertEntityMappingAsync(entityMapping);
        }

        #endregion

        /// <summary>
        /// returns the product corresponding to sku, will return deleted products
        /// </summary>
        /// <param name="sku"></param>
        public Product GetExistingProductBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _productRepository.Table
                        orderby p.Id
                        where p.Sku == sku
                        select p;
            var product = query.FirstOrDefault();
            return product;
        }

        public async Task<SpecificationAttribute> GetCategorySpecificationAttributeAsync()
        {
            SpecificationAttribute categoryAttribute = _specificationAttributeRepository.Table
                .Where(sar => sar.Name.ToLower() == "category")
                .Select(sar => sar).FirstOrDefault();

            if (categoryAttribute == null)
            {
                categoryAttribute = new SpecificationAttribute();
                categoryAttribute.Name = "Category";
                categoryAttribute.DisplayOrder = -1;
                await _specificationAttributeRepository.InsertAsync(categoryAttribute);
            }

            return categoryAttribute;
        }

        #region product attribute utilities

        public async Task InsertProductAttributeMappingAsync(
            int productId,
            int attributeId,
            EntityManager<ProductAttributeMapping> attributeManager)
        {
            ProductAttributeMapping productAttributeMapping = new ProductAttributeMapping();
            productAttributeMapping.ProductId = productId;
            productAttributeMapping.ProductAttributeId = attributeId;
            productAttributeMapping.AttributeControlType = AttributeControlType.MultilineTextbox;
            productAttributeMapping.IsRequired = false;
            await attributeManager.InsertAsync(productAttributeMapping);
        }

        public async Task<ProductAttribute> GetPickupAttributeAsync()
        {
            return await GetProductAttributeByNameAsync("Pickup");
        }

        public async Task<ProductAttribute> GetHomeDeliveryAttributeAsync()
        {
            return await GetProductAttributeByNameAsync("Home Delivery");
        }

        public async Task<PredefinedProductAttributeValue> GetHomeDeliveryAttributeValueAsync()
        {
            ProductAttribute hdAttribute = await GetHomeDeliveryAttributeAsync();
            PredefinedProductAttributeValue hdDefaultAttribute
                = (await _productAttributeService.GetPredefinedProductAttributeValuesAsync(hdAttribute.Id)).FirstOrDefault();

            if (hdDefaultAttribute == null)
            {
                PredefinedProductAttributeValue ppav = new PredefinedProductAttributeValue();
                ppav.Name = "This item will be delivered to you by ABC Warehouse";
                ppav.IsPreSelected = true;
                ppav.PriceAdjustment = 0;
                ppav.ProductAttributeId = hdAttribute.Id;
                await _productAttributeService.InsertPredefinedProductAttributeValueAsync(ppav);
                hdDefaultAttribute = ppav;
            }
            return hdDefaultAttribute;
        }

        private async Task<ProductAttribute> GetProductAttributeByNameAsync(string name)
        {
            ProductAttribute productAttribute =
                (await _productAttributeService.GetAllProductAttributesAsync())
                .Where(p => p.Name == name)
                .Select(p => p).SingleOrDefault();

            if (productAttribute == null)
            {
                productAttribute = new ProductAttribute();
                productAttribute.Name = name;
                await _productAttributeService.InsertProductAttributeAsync(productAttribute);
            }
            return productAttribute;
        }

        public Product CoreClone(Product original)
        {
            var product = new Product();
            product.DisableBuyButton = original.DisableBuyButton;
            product.Height = original.Height;
            product.Length = original.Length;
            product.ManufacturerPartNumber = original.ManufacturerPartNumber;
            product.Name = original.Name;
            product.OldPrice = original.OldPrice;
            product.Price = original.Price;
            product.ShortDescription = original.ShortDescription;
            product.FullDescription = original.FullDescription;
            product.Sku = original.Sku;
            product.Gtin = original.Gtin;
            product.Weight = original.Weight;
            product.Width = original.Width;
            product.LimitedToStores = original.LimitedToStores;
            product.Published = original.Published;
            product.IsShipEnabled = original.IsShipEnabled;

            return product;
        }

        public bool CoreEquals(Product p1, Product p2)
        {
            if (p1 == null || p2 == null)
                return false;
            return p1.DisableBuyButton == p2.DisableBuyButton &&
            p1.Height == p2.Height &&
            p1.Length == p2.Length &&
            p1.ManufacturerPartNumber == p2.ManufacturerPartNumber &&
            p1.Name == p2.Name &&
            p1.OldPrice == p2.OldPrice &&
            p1.Price == p2.Price &&
            p1.ShortDescription == p2.ShortDescription &&
            p1.FullDescription == p2.FullDescription &&
            p1.Sku == p2.Sku &&
            p1.Gtin == p2.Gtin &&
            p1.Weight == p2.Weight &&
            p1.Width == p2.Width &&
            p1.LimitedToStores == p2.LimitedToStores &&
            p1.Published == p2.Published &&
            p1.IsShipEnabled == p2.IsShipEnabled;
        }

        #endregion

        /// <summary>
        ///		Add a product override onto the widget with the given ID
        ///		for the product with the given ID.
        ///		This will allow the widget to appear on the product's page.
        /// </summary>
        /// <param name="widgetId">
        ///		The ID of the widget to which you want to add a product.
        /// </param>
        /// <param name="productId">
        ///		The ID of product for which you want the widget to display.
        /// </param>
        public async Task AddProductToHtmlWidgetAsync(int widgetId, int productId)
        {
            var condition = _entityConditionRepository.Table.Where
                (
                    ec => (ec.EntityType == EntityType.HtmlWidget) &&
                        (ec.EntityId == widgetId)
                )
                .FirstOrDefault();

            ProductOverride productOverride = new ProductOverride();
            productOverride.ConditionId = condition.Id;
            productOverride.ProductId = productId;
            productOverride.ProductState = OverrideState.Include;
            await _productOverrideRepository.InsertAsync(productOverride);

            return;
        }
    }
}