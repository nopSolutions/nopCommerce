using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Core.Web;

namespace Nop.Core.Plugins.Scheduling
{
    /// <summary>
    /// Maintains a list of scheduler actions and checks wether it's time to 
    /// execute them.
    /// </summary>
    [Service]
    public class Scheduler : IAutoStart
    {
        IList<ScheduledAction> actions;
        IHeart heart;
        readonly IWorker worker;
        Web.IWebContext context;
        IErrorHandler errorHandler;
        IEngine engine;

        public Scheduler(IEngine engine, IPluginFinder plugins, IHeart heart, IWorker worker, IWebContext context, IErrorHandler errorHandler)
        {
            this.engine = engine;
            actions = new List<ScheduledAction>(InstantiateActions(plugins));
            this.heart = heart;
            this.worker = worker;
            this.context = context;
            this.errorHandler = errorHandler;
        }

        public IList<ScheduledAction> Actions
        {
            get { return actions; }
        }

        protected TimeSpan CalculateInterval(int interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotImplementedException("Unknown time unit: " + unit);
            }
        }

        private IEnumerable<ScheduledAction> InstantiateActions(IPluginFinder plugins)
        {
            foreach (ScheduleExecutionAttribute attr in plugins.GetPlugins<ScheduleExecutionAttribute>())
            {
                ScheduledAction action = Activator.CreateInstance(attr.Decorates) as ScheduledAction;
                action.Interval = CalculateInterval(attr.Interval, attr.Unit);
                action.Repeat = attr.Repeat;
                yield return action;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void heart_Beat(object sender, EventArgs e)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                ScheduledAction action = actions[i];
                if (action.ShouldExecute())
                {
                    action.IsExecuting = true;
                    worker.DoWork(delegate
                    {
                        try
                        {
                            Debug.WriteLine("Executing " + action.GetType().Name);
                            action.Engine = engine;
                            action.Execute();
                            action.ErrorCount = 0;
                        }
                        catch (Exception ex)
                        {
                            action.ErrorCount++;
                            action.OnError(ex);     // wayne: call custom action error handler
                        }
                        finally
                        {
                            try
                            {
                                IClosable closable = action as IClosable;
                                if (closable != null)
                                    closable.Dispose();
                            }
                            catch (Exception ex)
                            {
                                errorHandler.Notify(ex);
                            }
                        }
                        action.LastExecuted = DateTime.Now;
                        action.IsExecuting = false;

                        try
                        {
                            context.Close();
                        }
                        catch (Exception ex)
                        {
                            errorHandler.Notify(ex);
                        }
                    });

                    if (action.Repeat == Repeat.Once)
                    {
                        actions.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        #region IStartable Members

        public void Start()
        {
            heart.Beat += new EventHandler(heart_Beat);
        }

        public void Stop()
        {
            heart.Beat -= new EventHandler(heart_Beat);
        }

        #endregion
    }
}
