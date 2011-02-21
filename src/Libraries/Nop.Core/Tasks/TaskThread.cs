using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Xml;

namespace Nop.Core.Tasks 
{
    /// <summary>
    /// Represents task thread
    /// </summary>
    public partial class TaskThread : IDisposable
    {
        private Timer _timer;
        private bool _disposed;
        private DateTime _started;
        private bool _isRunning;
        private readonly Dictionary<string, Task> _tasks;
        private readonly int _seconds;

        private TaskThread()
        {
            this._tasks = new Dictionary<string, Task>();
            this._seconds = 10 * 60;
        }

        internal TaskThread(XmlNode node)
        {
            this._tasks = new Dictionary<string, Task>();
            this._seconds = 10 * 60;
            this._isRunning = false;
            if ((node.Attributes["seconds"] != null) && !int.TryParse(node.Attributes["seconds"].Value, out this._seconds))
            {
                this._seconds = 10 * 60;
            }
        }

        private void Run()
        {
            this._started = DateTime.Now;
            this._isRunning = true;
            foreach (Task task in this._tasks.Values)
            {
                task.Execute();
            }
            this._isRunning = false;
        }

        private void TimerHandler(object state)
        {
            this._timer.Change(-1, -1);
            this.Run();
            this._timer.Change(this.Interval, this.Interval);
        }

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Dispose()
        {
            if ((this._timer != null) && !this._disposed)
            {
                lock (this)
                {
                    this._timer.Dispose();
                    this._timer = null;
                    this._disposed = true;
                }
            }
        }

        /// <summary>
        /// Inits a timer
        /// </summary>
        public void InitTimer()
        {
            if (this._timer == null)
            {
                this._timer = new Timer(new TimerCallback(this.TimerHandler), null, this.Interval, this.Interval);
            }
        }

        /// <summary>
        /// Adds a task to the thread
        /// </summary>
        /// <param name="task">The task to be added</param>
        public void AddTask(Task task)
        {
            if (!this._tasks.ContainsKey(task.Name))
            {
                this._tasks.Add(task.Name, task);
            }
        }


        /// <summary>
        /// Gets the interval in seconds at which to run the tasks
        /// </summary>
        public int Seconds
        {
            get
            {
                return this._seconds;
            }
        }

        /// <summary>
        /// Get a datetime when thread has been started
        /// </summary>
        public DateTime Started
        {
            get
            {
                return this._started;
            }
        }

        /// <summary>
        /// Get a value indicating whether thread is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this._isRunning;
            }
        }

        /// <summary>
        /// Get a list of tasks
        /// </summary>
        public IList<Task> Tasks
        {
            get
            {
                var list = new List<Task>();
                foreach (var task in this._tasks.Values)
                {
                    list.Add(task);
                }
                return new ReadOnlyCollection<Task>(list);
            }
        }

        /// <summary>
        /// Gets the interval at which to run the tasks
        /// </summary>
        public int Interval
        {
            get
            {
                return this._seconds * 1000;
            }
        }
    }
}
