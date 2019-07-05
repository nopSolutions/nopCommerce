using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Common;

namespace Nop.Web.Extensions
{
    public static class HtmlExtensions
    {
        //we have two pagers:
        //The first one can have custom routes
        //The second one just adds query string parameter
        public static IHtmlContent Pager<TModel>(this IHtmlHelper<TModel> html, PagerModel model)
        {
            if (model.TotalRecords == 0)
                return new HtmlString("");

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var links = new StringBuilder();
            if (model.ShowTotalSummary && (model.TotalPages > 0))
            {
                links.Append("<li class=\"total-summary\">");
                links.Append(string.Format(model.CurrentPageText, model.PageIndex + 1, model.TotalPages, model.TotalRecords));
                links.Append("</li>");
            }
            if (model.ShowPagerItems && (model.TotalPages > 1))
            {
                if (model.ShowFirst)
                {
                    //first page
                    if ((model.PageIndex >= 3) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                    {
                        model.RouteValues.pageNumber = 1;

                        links.Append("<li class=\"first-page\">");
                        if (model.UseRouteLinks)
                        {
                            var link = html.RouteLink(model.FirstButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.FirstPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        else
                        {
                            var link = html.ActionLink(model.FirstButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.FirstPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        links.Append("</li>");
                    }
                }
                if (model.ShowPrevious)
                {
                    //previous page
                    if (model.PageIndex > 0)
                    {
                        model.RouteValues.pageNumber = (model.PageIndex);

                        links.Append("<li class=\"previous-page\">");
                        if (model.UseRouteLinks)
                        {
                            var link = html.RouteLink(model.PreviousButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.PreviousPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        else
                        {
                            var link = html.ActionLink(model.PreviousButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.PreviousPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        links.Append("</li>");
                    }
                }
                if (model.ShowIndividualPages)
                {
                    //individual pages
                    var firstIndividualPageIndex = model.GetFirstIndividualPageIndex();
                    var lastIndividualPageIndex = model.GetLastIndividualPageIndex();
                    for (var i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                    {
                        if (model.PageIndex == i)
                        {
                            links.AppendFormat("<li class=\"current-page\"><span>{0}</span></li>", (i + 1));
                        }
                        else
                        {
                            model.RouteValues.pageNumber = (i + 1);

                            links.Append("<li class=\"individual-page\">");
                            if (model.UseRouteLinks)
                            {
                                var link = html.RouteLink((i + 1).ToString(), model.RouteActionName, model.RouteValues, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), (i + 1)) });
                                links.Append(link.ToHtmlString());
                            }
                            else
                            {
                                var link = html.ActionLink((i + 1).ToString(), model.RouteActionName, model.RouteValues, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), (i + 1)) });
                                links.Append(link.ToHtmlString());
                            }
                            links.Append("</li>");
                        }
                    }
                }
                if (model.ShowNext)
                {
                    //next page
                    if ((model.PageIndex + 1) < model.TotalPages)
                    {
                        model.RouteValues.pageNumber = (model.PageIndex + 2);

                        links.Append("<li class=\"next-page\">");
                        if (model.UseRouteLinks)
                        {
                            var link = html.RouteLink(model.NextButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.NextPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        else
                        {
                            var link = html.ActionLink(model.NextButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.NextPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        links.Append("</li>");
                    }
                }
                if (model.ShowLast)
                {
                    //last page
                    if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                    {
                        model.RouteValues.pageNumber = model.TotalPages;

                        links.Append("<li class=\"last-page\">");
                        if (model.UseRouteLinks)
                        {
                            var link = html.RouteLink(model.LastButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.LastPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        else
                        {
                            var link = html.ActionLink(model.LastButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.LastPageTitle") });
                            links.Append(link.ToHtmlString());
                        }
                        links.Append("</li>");
                    }
                }
            }
            var result = links.ToString();
            if (!string.IsNullOrEmpty(result))
            {
                result = "<ul>" + result + "</ul>";
            }
            return new HtmlString(result);
        }

        public static IHtmlContent ForumTopicSmallPager<TModel>(this IHtmlHelper<TModel> html, ForumTopicRowModel model)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var forumTopicId = model.Id;
            var forumTopicSlug = model.SeName;
            var totalPages = model.TotalPostPages;

            if (totalPages > 0)
            {
                var links = new StringBuilder();

                if (totalPages <= 4)
                {
                    for (var x = 1; x <= totalPages; x++)
                    {
                        var link = html.RouteLink(x.ToString(), "TopicSlugPaged", new { id = forumTopicId, pageNumber = x, slug = forumTopicSlug }, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), x.ToString()) });
                        links.Append(link.ToHtmlString());
                        if (x < totalPages)
                        {
                            links.Append(", ");
                        }
                    }
                }
                else
                {
                    var link1 = html.RouteLink("1", "TopicSlugPaged", new { id = forumTopicId, pageNumber = 1, slug = forumTopicSlug }, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), 1) });
                    links.Append(link1.ToHtmlString());
                    links.Append(" ... ");

                    for (var x = (totalPages - 2); x <= totalPages; x++)
                    {
                        var link2 = html.RouteLink(x.ToString(), "TopicSlugPaged", new { id = forumTopicId, pageNumber = x, slug = forumTopicSlug }, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), x.ToString()) });
                        links.Append(link2.ToHtmlString());

                        if (x < totalPages)
                        {
                            links.Append(", ");
                        }
                    }
                }

                // Inserts the topic page links into the localized string ([Go to page: {0}])
                return new HtmlString(string.Format(localizationService.GetResource("Forum.Topics.GotoPostPager"), links));
            }
            return new HtmlString(string.Empty);
        }

        public static Pager Pager(this IHtmlHelper helper, IPageableModel pagination)
        {
            return new Pager(pagination, helper.ViewContext);
        }

        /// <summary>
        /// Get topic SEO name
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="html">HTML helper</param>
        /// <param name="systemName">System name</param>
        /// <returns>Topic SEO Name</returns>
        public static string GetTopicSeName<T>(this IHtmlHelper<T> html, string systemName)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();

            //static cache manager
            var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            var cacheKey = string.Format(NopModelCacheDefaults.TopicSenameBySystemName,
                systemName, workContext.WorkingLanguage.Id, storeContext.CurrentStore.Id,
                string.Join(",", workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedSeName = cacheManager.Get(cacheKey, () =>
            {
                var topicService = EngineContext.Current.Resolve<ITopicService>();
                var topic = topicService.GetTopicBySystemName(systemName, storeContext.CurrentStore.Id);
                if (topic == null)
                    return "";

                var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
                return urlRecordService.GetSeName(topic);
            });
            return cachedSeName;
        }

        /// <summary>
        /// Get topic title
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="html">HTML helper</param>
        /// <param name="systemName">System name</param>
        /// <returns>Topic SEO Name</returns>
        public static string GetTopicTitle<T>(this IHtmlHelper<T> html, string systemName)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();

            //static cache manager
            var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            var cacheKey = string.Format(NopModelCacheDefaults.TopicTitleBySystemName,
                systemName, workContext.WorkingLanguage.Id, storeContext.CurrentStore.Id,
                string.Join(",", workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedTitle = cacheManager.Get(cacheKey, () =>
            {
                var topicService = EngineContext.Current.Resolve<ITopicService>();
                var topic = topicService.GetTopicBySystemName(systemName, storeContext.CurrentStore.Id);
                if (topic == null)
                    return "";

                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                return localizationService.GetLocalized(topic, x => x.Title);
            });
            return cachedTitle;
        }
    }
}