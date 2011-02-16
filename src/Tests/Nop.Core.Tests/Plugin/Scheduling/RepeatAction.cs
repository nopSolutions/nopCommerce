using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins.Scheduling;

namespace Nop.Core.Tests.Plugin.Scheduling
{
    [ScheduleExecution(60, Repeat=Repeat.Indefinitely)]
    public class RepeatAction : OnceAction
    {
    }
}
