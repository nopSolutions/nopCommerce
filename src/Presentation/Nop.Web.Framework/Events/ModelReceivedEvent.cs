using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Services.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Events;

public partial interface IModelReceivedEventConsumer<T> : IConsumer<ModelReceivedEvent<BaseNopModel>>
where T : BaseNopModel
{
    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    async Task IConsumer<ModelReceivedEvent<BaseNopModel>>.HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
    {
        var castedModel = eventMessage.Model as T;

        if (castedModel == null)
            return;

        await HandleEventAsync(new ModelReceivedEvent<T>(castedModel, eventMessage.ModelState));
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task HandleEventAsync(ModelReceivedEvent<T> eventMessage);
}

/// <summary>
/// Represents an event that occurs after the model is received from the view
/// </summary>
/// <typeparam name="T">Type of the model</typeparam>
public partial class ModelReceivedEvent<T>
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="model">Model</param>
    /// <param name="modelState">Model state</param>
    public ModelReceivedEvent(T model, ModelStateDictionary modelState)
    {
        Model = model;
        ModelState = modelState;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a model
    /// </summary>
    public T Model { get; protected set; }

    /// <summary>
    /// Gets a model state
    /// </summary>
    public ModelStateDictionary ModelState { get; protected set; }

    #endregion
}