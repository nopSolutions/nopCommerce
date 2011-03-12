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

        public static MvcHtmlString Hint(this HtmlHelper helper, string value)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Add attributes
            builder.MergeAttribute("src", ResolveUrl(helper, "/Areas/Admin/Content/images/ico-help.gif"));
            builder.MergeAttribute("alt", value);
            builder.MergeAttribute("title", value);

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
                .Title(EngineContext.Current.LocalizationService().GetResource("Admin.Common.AreYouSure"))
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

        public static MvcHtmlString NopLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var hintResource = string.Empty;
            object value = null;
            if (metadata.AdditionalValues.TryGetValue("NopResourceDisplayName", out value))
            {
                var resourceDisplayName = value as NopResourceDisplayName;
                if (resourceDisplayName != null)
                {
                    hintResource =
                        EngineContext.Current.Resolve<ILocalizationService>().GetResource(
                            resourceDisplayName.ResourceKey + ".Hint",
                            EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id, false, "");

                    result.Append(helper.Hint(hintResource).ToHtmlString());
                }
            }
            result.Append(helper.LabelFor(expression, new {title=hintResource}));
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

    }
}

