﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Events;
using Nop.Core.Http.Extensions;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that publish ModelReceived event before the action executes, after model binding is complete
/// and publish ModelPrepared event after the action executes, before the action result
/// </summary>
public sealed class PublishModelEventsAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public PublishModelEventsAttribute(bool ignore = false) : base(typeof(PublishModelEventsFilter))
    {
        IgnoreFilter = ignore;
        Arguments = [ignore];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents filter that publish ModelReceived event before the action executes, after model binding is complete
    /// and publish ModelPrepared event after the action executes, before the action result
    /// </summary>
    private class PublishModelEventsFilter : IAsyncActionFilter, IAsyncResultFilter
    {
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public PublishModelEventsFilter(bool ignoreFilter,
            IEventPublisher eventPublisher)
        {
            _ignoreFilter = ignoreFilter;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Whether to ignore this filter
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>Result</returns>
        protected virtual bool IgnoreFilter(FilterContext context)
        {
            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<PublishModelEventsAttribute>()
                .FirstOrDefault();

            return actionFilter?.IgnoreFilter ?? _ignoreFilter;
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PublishModelReceivedEventAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            //only in POST requests
            if (!context.HttpContext.Request.IsPostRequest())
                return;

            if (IgnoreFilter(context))
                return;

            //model received event
            foreach (var model in context.ActionArguments.Values.OfType<BaseNopModel>())
            {
                //we publish the ModelReceived event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelReceived<BaseNopModel>> interface to handle this event
                await _eventPublisher.ModelReceivedAsync(model, context.ModelState);
            }
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PublishModelPreparedEventAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (IgnoreFilter(context))
                return;

            //model prepared event
            if (context.Controller is Controller controller)
                await _eventPublisher.ModelPreparedAsync(controller.ViewData.Model);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await PublishModelReceivedEventAsync(context);
            if (context.Result == null)
                await next();
            await PublishModelPreparedEventAsync(context);
        }

        /// <summary>Called asynchronously before the action result.</summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (IgnoreFilter(context))
                return;

            //model prepared event
            if (context.Result is JsonResult result)
                await _eventPublisher.ModelPreparedAsync(result.Value);

            await next();
        }

        #endregion
    }

    #endregion
}