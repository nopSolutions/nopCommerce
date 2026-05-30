using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Integration.Pos;

// Thin HTTP adapter — no own state. AllocationSettings is installed by AllocationGatePlugin
// (shared via project reference); no schedule tasks to register here.
public class PosIntegrationPlugin : BasePlugin, IMiscPlugin
{
}
