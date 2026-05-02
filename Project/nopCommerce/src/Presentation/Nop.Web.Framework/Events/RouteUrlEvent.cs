using Microsoft.AspNetCore.Routing;
using Nop.Core.Events;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs when a generic URL for the specified entity type and route values generated
/// </summary>
public partial class RouteUrlEvent : IStopProcessingEvent
{
    #region Ctor

    public RouteUrlEvent(Type entityType, RouteValueDictionary values, string protocol, string host, string fragment)
    {
        EntityType = entityType;
        Values = values;
        Protocol = protocol;
        Host = host;
        Fragment = fragment;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets entity type that is used to generate URL
    /// </summary>
    public Type EntityType { get; protected set; }

    /// <summary>
    /// Gets an object that contains route values
    /// </summary>
    public RouteValueDictionary Values { get; protected set; }

    /// <summary>
    /// Gets protocol for the URL
    /// </summary>
    public string Protocol { get; protected set; }

    /// <summary>
    /// Gets host name for the URL
    /// </summary>
    public string Host { get; protected set; }

    /// <summary>
    /// Gets fragment for the URL
    /// </summary>
    public string Fragment { get; protected set; }

    /// <summary>
    /// Gets or sets the generated URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    public bool StopProcessing { get; set; }

    #endregion
}