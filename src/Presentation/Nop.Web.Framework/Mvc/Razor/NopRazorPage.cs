﻿using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework.Mvc.Razor;

/// <summary>
/// Web view page
/// </summary>
/// <typeparam name="TModel">Model</typeparam>
public abstract partial class NopRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
{
    protected ILocalizationService _localizationService;
    protected Localizer _localizer;

    /// <summary>
    /// Get a localized resources
    /// </summary>
    public Localizer T
    {
        get
        {
            _localizationService ??= EngineContext.Current.Resolve<ILocalizationService>();

            _localizer ??= (format, args) =>
            {
                var resFormat = _localizationService.GetResourceAsync(format).Result;
                if (string.IsNullOrEmpty(resFormat))
                {
                    return new LocalizedString(format);
                }
                return new LocalizedString((args == null || args.Length == 0)
                    ? resFormat
                    : string.Format(resFormat, args));
            };
            return _localizer;
        }
    }
}