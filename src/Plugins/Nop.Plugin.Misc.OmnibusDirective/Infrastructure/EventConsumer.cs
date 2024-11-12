using Nop.Services.Events;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.OmnibusDirective.Infrastructure;

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
                SystemName = "EU Omnibus Directive",
                Title = "Omnibus Directive plugin",
                Url = eventMessage.GetMenuItemUrl("OmnibusDirective", "Configure"),
                IconClass = "far fa-dot-circle",
                Visible = true,
            });

        return Task.CompletedTask;
    }
}
