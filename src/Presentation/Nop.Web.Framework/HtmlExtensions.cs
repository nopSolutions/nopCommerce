using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.WebPages;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;
using Telerik.Web.Mvc.UI;
using System.Web.Mvc.Html;
namespace Nop.Web.Framework
{
    public static class HtmlExtensions
    {
        public static string ResolveUrl(this HtmlHelper helper, string url)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            return urlHelper.Content(url);
        }
        public static MvcHtmlString Hint(this HtmlHelper helper, string resourceName)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Add attributes
            var resource = EngineContext.Current.Resolve<Services.Localization.ILocalizationService>().GetResource(
                    resourceName);
            builder.MergeAttribute("src", ResolveUrl(helper, "/Areas/Admin/Content/images/ico-help.gif"));
            builder.MergeAttribute("alt", resource);
            builder.MergeAttribute("title", resource);

            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this HtmlHelper<T> helper, string name,
             Func<int, HelperResult> localizedTemplate,
             Func<T, HelperResult> standardTemplate)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
            {
                if (helper.ViewData.Model.Locales.Count > 1)
                {
                    var tabStrip = helper.Telerik().TabStrip().Name(name).Items(x =>
                                                                                                  {
                                                                                                      x.Add().Text("Standard").Content(standardTemplate(helper.ViewData.Model).ToHtmlString()).Selected(true);
                                                                                                      for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                                                                                                      {
                                                                                                          var locale = helper.ViewData.Model.Locales[i];
                                                                                                          x.Add().Text(locale.Language.Name)
                                                                                                             .Content(localizedTemplate
                                                                                                                  (i).
                                                                                                                  ToHtmlString
                                                                                                                  ())
                                                                                                             .ImageUrl("~/Content/images/flags/" + locale.Language.FlagImageFileName);
                                                                                                      }
                                                                                                  }).ToHtmlString();
                    writer.Write(tabStrip);
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector = null) where T : BaseNopEntityModel
        {
            var modalId = helper.DeleteConfirmationModelId().ToHtmlString();

            #region Write click events for button, if supplied
            
            if (!string.IsNullOrEmpty(buttonsSelector))
            {
                var textWriter = new StringWriter();
                IClientSideObjectWriter objectWriter = new ClientSideObjectWriterFactory().Create(buttonsSelector, "click", textWriter);
                objectWriter.Start();
                textWriter.Write("function(e){e.preventDefault();openModalWindow(\"" + modalId + "\");}");
                objectWriter.Complete();
                var value = textWriter.ToString();
                ScriptRegistrar.Current.OnDocumentReadyStatements.Add(value);
            }

            #endregion

            var window = helper.Telerik().Window().Name(modalId)
                .Title("Are you sure?")
                .Modal(true)
                .Effects(x => x.Toggle())
                .Resizable(x => x.Enabled(false))
                .Buttons(x => x.Close())
                .Visible(false)
                .Content(helper.Partial("Delete", helper.ViewData.Model).ToHtmlString()).ToHtmlString();

            return MvcHtmlString.Create(window);
        }

        public static MvcHtmlString DeleteConfirmationModelId<T>(this HtmlHelper<T> helper) where T : BaseNopEntityModel
        {
            return MvcHtmlString.Create(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation");
        }

    }
}

