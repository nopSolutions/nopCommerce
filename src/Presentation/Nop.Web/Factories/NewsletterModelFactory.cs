using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Localization;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the newsletter model factory
/// </summary>
public partial class NewsletterModelFactory : INewsletterModelFactory
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public NewsletterModelFactory(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ILocalizationService localizationService)
    {
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _localizationService = localizationService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the newsletter box model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter box model
    /// </returns>
    public virtual Task<NewsletterBoxModel> PrepareNewsletterBoxModelAsync()
    {
        var model = new NewsletterBoxModel
        {
            AllowToUnsubscribe = _customerSettings.NewsletterBlockAllowToUnsubscribe,
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsletterPage,
            IsReCaptchaV3 = _captchaSettings.CaptchaType == CaptchaType.ReCaptchaV3,
            ReCaptchaPublicKey = _captchaSettings.ReCaptchaPublicKey
        };

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the subscription activation model
    /// </summary>
    /// <param name="active">Whether the subscription has been activated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the subscription activation model
    /// </returns>
    public virtual async Task<SubscriptionActivationModel> PrepareSubscriptionActivationModelAsync(bool active)
    {
        var model = new SubscriptionActivationModel
        {
            Result = active
                ? await _localizationService.GetResourceAsync("Newsletter.ResultActivated")
                : await _localizationService.GetResourceAsync("Newsletter.ResultDeactivated")
        };

        return model;
    }

    #endregion
}