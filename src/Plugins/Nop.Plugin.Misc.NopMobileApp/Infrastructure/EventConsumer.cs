using Nop.Services.Events;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.NopMobileApp.Infrastructure;

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
                SystemName = "nopCommerce mobile application",
                Title = "Mobile App",
                Url = eventMessage.GetMenuItemUrl("NopMobileApp", "Configure"),
                IconClass = "far fa-dot-circle",
                Visible = true
            });

        return Task.CompletedTask;
    }
}
