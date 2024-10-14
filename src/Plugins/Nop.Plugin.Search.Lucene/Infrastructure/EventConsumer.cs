using Nop.Services.Events;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Search.Lucene.Infrastructure;

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
                SystemName = "Lucene integration",
                Title = "Lucene search provider",
                Url = eventMessage.GetMenuItemUrl("Lucene", "Configure"),
                IconClass = "far fa-dot-circle",
                Visible = true
            });

        return Task.CompletedTask;
    }
}
