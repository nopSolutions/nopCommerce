using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class BlogMonthsViewComponent : NopViewComponent
{
    protected readonly BlogSettings _blogSettings;
    protected readonly IBlogModelFactory _blogModelFactory;

    public BlogMonthsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory)
    {
        _blogSettings = blogSettings;
        _blogModelFactory = blogModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="currentCategoryId">The current category identifier</param>
    /// <param name="currentProductId">The current product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
    {
        if (!_blogSettings.Enabled)
            return Content("");

        var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
        return await ViewAsync(model);
    }
}