using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins.Scheduling;

namespace Nop.Core.Tests.Plugin.Scheduling
{
    [ScheduleExecution(Repeat.Once)]
    public class OnceAction : ScheduledAction
    {
        public int executions = 0;
        public DateTime LastCall;

        public override void Execute()
        {
            executions++;
            LastCall = CommonHelper.CurrentTime();
        }
    }
}
