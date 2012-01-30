using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Common;

namespace Nop.Web.Extensions
{
    public static class PagerHtmlExtension
    {
        //we have two pagers:
        //The first one can have custom routes
        //The second one just adds query string parameter
        public static MvcHtmlString Pager<TModel>(this HtmlHelper<TModel> html, PagerModel model)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var links = new StringBuilder();

            if (model.ShowTotalSummary && (model.TotalPages > 0))
            {
                links.Append(string.Format(model.CurrentPageText, model.PageIndex + 1, model.TotalPages, model.TotalRecords));
                links.Append("&nbsp;");
            }
            if (model.ShowPagerItems && (model.TotalPages > 1))
            {
                if (model.ShowFirst)
                {
                    if ((model.PageIndex >= 3) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                    {
                        if (model.ShowIndividualPages)
                        {
                            links.Append("&nbsp;");
                        }

                        model.RouteValues.page = 1;

                        if (model.UseRouteLinks)
                        {
                            links.Append(html.RouteLink(model.FirstButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.FirstPageTitle") }));
                        }
                        else
                        {
                            links.Append(html.ActionLink(model.FirstButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.FirstPageTitle") }));
                        }

                        if ((model.ShowIndividualPages || (model.ShowPrevious && (model.PageIndex > 0))) || model.ShowLast)
                        {
                            links.Append("&nbsp;...&nbsp;");
                        }
                    }
                }
                if (model.ShowPrevious)
                {
                    if (model.PageIndex > 0)
                    {
                        model.RouteValues.page = (model.PageIndex);

                        if (model.UseRouteLinks)
                        {
                            links.Append(html.RouteLink(model.PreviousButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.PreviousPageTitle") }));
                        }
                        else
                        {
                            links.Append(html.ActionLink(model.PreviousButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.PreviousPageTitle") }));
                        }

                        if ((model.ShowIndividualPages || model.ShowLast) || (model.ShowNext && ((model.PageIndex + 1) < model.TotalPages)))
                        {
                            links.Append("&nbsp;");
                        }
                    }
                }
                if (model.ShowIndividualPages)
                {
                    int firstIndividualPageIndex = model.GetFirstIndividualPageIndex();
                    int lastIndividualPageIndex = model.GetLastIndividualPageIndex();
                    for (int i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                    {
                        if (model.PageIndex == i)
                        {
                            links.AppendFormat("<span>{0}</span>", (i + 1).ToString());
                        }
                        else
                        {
                            model.RouteValues.page = (i + 1);

                            if (model.UseRouteLinks)
                            {
                                links.Append(html.RouteLink((i + 1).ToString(), model.RouteActionName, (object)model.RouteValues, new { title = String.Format(localizationService.GetResource("Pager.PageLinkTitle").ToString(), (i + 1).ToString()) }));
                            }
                            else
                            {
                                links.Append(html.ActionLink((i + 1).ToString(), model.RouteActionName, (object)model.RouteValues, new { title = String.Format(localizationService.GetResource("Pager.PageLinkTitle").ToString(), (i + 1).ToString()) }));
                            }
                        }
                        if (i < lastIndividualPageIndex)
                        {
                            links.Append("&nbsp;");
                        }
                    }
                }
                if (model.ShowNext)
                {
                    if ((model.PageIndex + 1) < model.TotalPages)
                    {
                        if (model.ShowIndividualPages)
                        {
                            links.Append("&nbsp;");
                        }

                        model.RouteValues.page = (model.PageIndex + 2);

                        if (model.UseRouteLinks)
                        {
                            links.Append(html.RouteLink(model.NextButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.NextPageTitle") }));
                        }
                        else
                        {
                            links.Append(html.ActionLink(model.NextButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.NextPageTitle") }));
                        }
                    }
                }
                if (model.ShowLast)
                {
                    if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                    {
                        if (model.ShowIndividualPages || (model.ShowNext && ((model.PageIndex + 1) < model.TotalPages)))
                        {
                            links.Append("&nbsp;...&nbsp;");
                        }

                        model.RouteValues.page = model.TotalPages;

                        if (model.UseRouteLinks)
                        {
                            links.Append(html.RouteLink(model.LastButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.LastPageTitle") }));
                        }
                        else
                        {
                            links.Append(html.ActionLink(model.LastButtonText, model.RouteActionName, (object)model.RouteValues, new { title = localizationService.GetResource("Pager.LastPageTitle") }));
                        }
                    }
                }
            }
            return MvcHtmlString.Create(links.ToString());
        }
        public static MvcHtmlString ForumTopicSmallPager<TModel>(this HtmlHelper<TModel> html, ForumTopicRowModel model)
        {
            var localizationService= EngineContext.Current.Resolve<ILocalizationService>();

            var forumTopicId = model.Id;
            var forumTopicSlug = model.SeName;
            var totalPages = model.TotalPostPages;

            if (totalPages > 0)
            {
                var links = new StringBuilder();

                if (totalPages <= 4)
                {
                    for (int x = 1; x <= totalPages; x++)
                    {
                        links.Append(html.RouteLink(x.ToString(), "TopicSlugPaged", new { id = forumTopicId, page = (x), slug = forumTopicSlug }, new { title = String.Format(localizationService.GetResource("Pager.PageLinkTitle"), x.ToString()) }));
                        if (x < totalPages)
                        {
                            links.Append(", ");
                        }
                    }
                }
                else
                {
                    links.Append(html.RouteLink("1", "TopicSlugPaged", new { id = forumTopicId, page = (1), slug = forumTopicSlug }, new { title = String.Format(localizationService.GetResource("Pager.PageLinkTitle"), 1) }));
                    links.Append(" ... ");

                    for (int x = (totalPages - 2); x <= totalPages; x++)
                    {
                        links.Append(html.RouteLink(x.ToString(), "TopicSlugPaged", new { id = forumTopicId, page = (x), slug = forumTopicSlug }, new { title = String.Format(localizationService.GetResource("Pager.PageLinkTitle"), x.ToString()) }));

                        if (x < totalPages)
                        {
                            links.Append(", ");
                        }
                    }
                }

                // Inserts the topic page links into the localized string ([Go to page: {0}])
                return MvcHtmlString.Create(String.Format(localizationService.GetResource("Forum.Topics.GotoPostPager"), links.ToString()));
            }
            return MvcHtmlString.Create(string.Empty);
        }

        public static Pager Pager(this HtmlHelper helper, IPageableModel pagination)
        {
            return new Pager(pagination, helper.ViewContext);
        }
        public static Pager Pager(this HtmlHelper helper, string viewDataKey)
        {
            var dataSource = helper.ViewContext.ViewData.Eval(viewDataKey) as IPageableModel;

            if (dataSource == null)
            {
                throw new InvalidOperationException(string.Format("Item in ViewData with key '{0}' is not an IPagination.",
                                                                  viewDataKey));
            }

            return helper.Pager(dataSource);
        }
    }
}
