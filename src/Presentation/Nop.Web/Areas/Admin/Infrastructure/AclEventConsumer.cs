using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Topics;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Infrastructure;

/// <summary>
/// Represents ACL CategoryModelReceived event consumer
/// </summary>
public partial class AclCategoryModelReceivedEventConsumer: IModelReceivedEventConsumer<CategoryModel>
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICategoryService _categoryService;

    #endregion

    #region Ctor

    public AclCategoryModelReceivedEventConsumer(IAclService aclService,
        ICategoryService categoryService)
    {
        _aclService = aclService;
        _categoryService = categoryService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelReceivedEvent<CategoryModel> eventMessage)
    {
        var categoryModel = eventMessage.Model;
        var category = await _categoryService.GetCategoryByIdAsync(categoryModel.Id);
        await _aclService.SaveAclAsync(category, categoryModel.SelectedCustomerRoleIds);
        var key = category == null ? categoryModel.Name : string.Empty;

        if (!string.IsNullOrEmpty(key))
            AclEventConsumer.TempData[key] = categoryModel.SelectedCustomerRoleIds;
    }

    #endregion
}

/// <summary>
/// Represents ACL ManufacturerModelReceived event consumer
/// </summary>
public partial class AclManufacturerModelReceivedEventConsumer: IModelReceivedEventConsumer<ManufacturerModel>
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly IManufacturerService _manufacturerService;

    #endregion

    #region Ctor

    public AclManufacturerModelReceivedEventConsumer(IAclService aclService,
        IManufacturerService manufacturerService)
    {
        _aclService = aclService;
        _manufacturerService = manufacturerService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelReceivedEvent<ManufacturerModel> eventMessage)
    {
        var manufacturerModel = eventMessage.Model;
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerModel.Id);
        await _aclService.SaveAclAsync(manufacturer, manufacturerModel.SelectedCustomerRoleIds);
        var key = manufacturer == null ? manufacturerModel.Name : string.Empty;

        if (!string.IsNullOrEmpty(key))
            AclEventConsumer.TempData[key] = manufacturerModel.SelectedCustomerRoleIds;
    }

    #endregion
}

/// <summary>
/// Represents ACL ProductModelReceived event consumer
/// </summary>
public partial class AclProductModelReceivedEventConsumer: IModelReceivedEventConsumer<ProductModel>
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly IProductService _productService;

    #endregion

    #region Ctor

    public AclProductModelReceivedEventConsumer(IAclService aclService,
        IProductService productService)
    {
        _aclService = aclService;
        _productService = productService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelReceivedEvent<ProductModel> eventMessage)
    {
        var productModel = eventMessage.Model;
        var product = await _productService.GetProductByIdAsync(productModel.Id);
        await _aclService.SaveAclAsync(product, productModel.SelectedCustomerRoleIds);
        var key = product == null ? productModel.Name : string.Empty;

        if (!string.IsNullOrEmpty(key))
            AclEventConsumer.TempData[key] = productModel.SelectedCustomerRoleIds;
    }

    #endregion
}

/// <summary>
/// Represents ACL TopicModelReceived event consumer
/// </summary>
public partial class AclTopicModelReceivedEventConsumer: IModelReceivedEventConsumer<TopicModel>
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ITopicService _topicService;

    #endregion

    #region Ctor

    public AclTopicModelReceivedEventConsumer(IAclService aclService,
        ITopicService topicService)
    {
        _aclService = aclService;
        _topicService = topicService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelReceivedEvent<TopicModel> eventMessage)
    {
        var topicModel = eventMessage.Model;
        var topic = await _topicService.GetTopicByIdAsync(topicModel.Id);
        await _aclService.SaveAclAsync(topic, topicModel.SelectedCustomerRoleIds);
        var key = topic == null ? topicModel.Title : string.Empty;

        if (!string.IsNullOrEmpty(key))
            AclEventConsumer.TempData[key] = topicModel.SelectedCustomerRoleIds;
    }

    #endregion
}

/// <summary>
/// Represents ACL event consumer
/// </summary>
public partial class AclEventConsumer : IConsumer<ModelPreparedEvent<IAclSupportedModel>>,
    IConsumer<EntityInsertedEvent<Manufacturer>>,
    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityInsertedEvent<Topic>>,
    IConsumer<EntityInsertedEvent<Category>>
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly IAclSupportedModelFactory _aclSupportedModelFactory;

    public static Dictionary<string, IList<int>> TempData { get; } = new(comparer: StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Ctor

    public AclEventConsumer(IAclService aclService,
        IAclSupportedModelFactory aclSupportedModelFactory)
    {
        _aclService = aclService;
        _aclSupportedModelFactory = aclSupportedModelFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Save ACL mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="key">Key to load mapping</param>
    /// <param name="entity">Entity to store mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task SaveStoredDataAsync<TEntity>(string key, TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
        if (!TempData.TryGetValue(key, out var value))
            return;

        await _aclService.SaveAclAsync(entity, value);
        TempData.Remove(key);
    }

    #endregion

    #region Methods
    
    /// <summary>
    /// Handle manufacturer inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle product inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle topic inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Topic> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Title, entity);
    }

    /// <summary>
    /// Handle category inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    #endregion

    public async Task HandleEventAsync(ModelPreparedEvent<IAclSupportedModel> eventMessage)
    {
        switch (eventMessage.Model)
        {
            case CategoryModel categoryModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(categoryModel, nameof(Category));
                break;
            case ManufacturerModel manufacturerModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(manufacturerModel, nameof(Manufacturer));
                break;
            case PluginModel pluginModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(pluginModel);
                break;
            case ProductModel productModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(productModel, nameof(Product));
                break;
            case TopicModel topicModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(topicModel, nameof(Topic));
                break;
            case CustomerSearchModel customerSearchModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(customerSearchModel);
                break;
        }
    }
}