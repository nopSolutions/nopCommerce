using Nop.Services.Events;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.PowerBI.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer: IConsumer<AdminMenuCreatedEvent>
{
    public Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        eventMessage.RootMenuItem.InsertBefore("Sales summary",
            new AdminMenuItem
            {
                SystemName = "Power BI integration",
                Title = "Power BI (advanced reports)",
                Url = eventMessage.GetMenuItemUrl("PowerBI", "Configure"),
                IconClass = "far fa-dot-circle",
                Visible = true
            });

        return Task.CompletedTask;
    }
}
