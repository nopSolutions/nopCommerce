using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
[ValidateVendor]
[SaveSelectedTab]
[NotNullValidationMessage]
public abstract partial class BaseAdminController : BaseController
{
    protected readonly IEventPublisher _eventPublisher;

    protected BaseAdminController(IEventPublisher eventPublisher = null)
    {
        _eventPublisher = eventPublisher ?? EngineContext.Current.Resolve<IEventPublisher>();
    }

    /// <summary>
    /// Create json result with passed data
    /// </summary>
    /// <param name="data">Data to create JsonResult</param>
    /// <returns>Created JsonResult</returns>
    protected virtual JsonResult CreateJsonResult(object data)
    {
        //use IsoDateFormat on writing JSON text to fix issue with dates in grid
        var useIsoDateFormat = EngineContext.Current.Resolve<AdminAreaSettings>()?.UseIsoDateFormatInJsonResult ?? false;
        var serializerSettings = EngineContext.Current.Resolve<IOptions<MvcNewtonsoftJsonOptions>>()?.Value?.SerializerSettings
            ?? new JsonSerializerSettings();

        if (!useIsoDateFormat)
            return base.Json(data, serializerSettings);

        serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

        return base.Json(data, serializerSettings);
    }
    
    /// <summary>
    /// Creates an object that serializes the specified object to JSON.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The created object that serializes the specified data to JSON format for the response.</returns>
    public virtual JsonResult Json<T>(T data)
    {
        if (data is not (BaseNopModel or IEnumerable<BaseNopModel>))
            return CreateJsonResult(data);

        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        eventPublisher.ModelPreparedAsync(data).Wait();

        return CreateJsonResult(data);
    }

    /// <summary>
    /// Creates an object that serializes the specified object to JSON.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The created object that serializes the specified data to JSON format for the response.</returns>
    public virtual async Task<JsonResult> JsonAsync<T>(T data)
    {
        if (data is not (BaseNopModel or IEnumerable<BaseNopModel>))
            return CreateJsonResult(data);

        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        await eventPublisher.ModelPreparedAsync(data);

        return CreateJsonResult(data);
    }
}