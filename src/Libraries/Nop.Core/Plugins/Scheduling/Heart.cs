using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;

namespace Nop.Core.Plugins.Scheduling
{
    /// <summary>
    /// A wrapper for a timer that beats at a certain interval.
    /// </summary>
    [Service(typeof(IHeart))]
    public class Heart : IAutoStart, IHeart
    {
        Timer timer;

        public Heart()
        {
            timer = new Timer(60 * 1000);
        }

        public Heart(Configuration.EngineSection config)
        {
            timer = new Timer(config.Scheduler.Interval * 1000);
        }

        public event EventHandler Beat;

        public void Start()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Beat: " + DateTime.Now);
            if (Beat != null)
                Beat(this, e);
        }

        public void Stop()
        {
            timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
            timer.Stop();
        }
    }
}
