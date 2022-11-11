using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute enabling CAPTCHA validation
    /// </summary>
    public sealed class ValidateCaptchaAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute 
        /// </summary>
        /// <param name="actionParameterName">The name of the action parameter to which the result will be passed</param>
        public ValidateCaptchaAttribute(string actionParameterName = "captchaValid") : base(typeof(ValidateCaptchaFilter))
        {
            Arguments = new object[] { actionParameterName };
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter enabling CAPTCHA validation
        /// </summary>
        private class ValidateCaptchaFilter : IAsyncActionFilter
        {
            #region Constants

            private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";
            private const string G_RESPONSE_FIELD_KEY = "g-recaptcha-response";

            #endregion

            #region Fields

            private readonly string _actionParameterName;
            private readonly CaptchaHttpClient _captchaHttpClient;
            private readonly CaptchaSettings _captchaSettings;
            private readonly ILogger _logger;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public ValidateCaptchaFilter(string actionParameterName,
                CaptchaHttpClient captchaHttpClient,
                CaptchaSettings captchaSettings,
                ILogger logger,
                IWorkContext workContext)
            {
                _actionParameterName = actionParameterName;
                _captchaHttpClient = captchaHttpClient;
                _captchaSettings = captchaSettings;
                _logger = logger;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task ValidateCaptchaAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //whether CAPTCHA is enabled
                if (_captchaSettings.Enabled && context.HttpContext?.Request != null)
                {
                    //push the validation result as an action parameter
                    var isValid = false;

                    //get form values
                    var captchaResponseValue = context.HttpContext.Request.Form[RESPONSE_FIELD_KEY];
                    var gCaptchaResponseValue = context.HttpContext.Request.Form[G_RESPONSE_FIELD_KEY];

                    if (!StringValues.IsNullOrEmpty(captchaResponseValue) || !StringValues.IsNullOrEmpty(gCaptchaResponseValue))
                    {
                        //validate request
                        try
                        {
                            var value = !StringValues.IsNullOrEmpty(captchaResponseValue) ? captchaResponseValue : gCaptchaResponseValue;
                            var response = await _captchaHttpClient.ValidateCaptchaAsync(value);

                            switch (_captchaSettings.CaptchaType)
                            {
                                case CaptchaType.CheckBoxReCaptchaV2:
                                    isValid = response.IsValid;
                                    break;

                                case CaptchaType.ReCaptchaV3:
                                    isValid = response.IsValid &&
                                        response.Action == context.RouteData.Values["action"].ToString() &&
                                        response.Score > _captchaSettings.ReCaptchaV3ScoreThreshold;
                                    break;

                                default:
                                    break;
                            }
                        }
                        catch (Exception exception)
                        {
                            await _logger.ErrorAsync("Error occurred on CAPTCHA validation", exception, await _workContext.GetCurrentCustomerAsync());
                        }
                    }

                    context.ActionArguments[_actionParameterName] = isValid;
                }
                else
                    context.ActionArguments[_actionParameterName] = false;
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
                await ValidateCaptchaAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}