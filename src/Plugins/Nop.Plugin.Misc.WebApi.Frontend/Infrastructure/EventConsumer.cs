using Nop.Services.Events;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.WebApi.Frontend.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer: IConsumer<AdminMenuCreatedEvent>
{
    public Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        eventMessage.RootMenuItem.InsertBefore("Local plugins",
            new AdminMenuItem
            {
                SystemName = "nopCommerce Web API plugin",
                Title = "Web API",
                Url = eventMessage.GetMenuItemUrl("WebApiFrontend", "Configure"),
                IconClass = "far fa-dot-circle",
                Visible = true
            });

        return Task.CompletedTask;
    }
}
