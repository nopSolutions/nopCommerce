using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class PrivateMessagesInboxViewComponent : NopViewComponent
{
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

    public PrivateMessagesInboxViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
    {
        _privateMessagesModelFactory = privateMessagesModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="pageNumber">Number of items page</param>
    /// <param name="tab">Tab name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
    {
        var model = await _privateMessagesModelFactory.PrepareInboxModelAsync(pageNumber, tab);
        return await ViewAsync(model);
    }
}