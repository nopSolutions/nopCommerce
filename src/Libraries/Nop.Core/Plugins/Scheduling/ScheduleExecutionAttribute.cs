using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins;

namespace Nop.Core.Plugins.Scheduling
{
    /// <summary>
    /// Defines how often a scheduled action should execute.
    /// </summary>
    public class ScheduleExecutionAttribute : Attribute, IPlugin
    {
        private string name = null;
        private int sortOrder = int.MaxValue;
        private Type decorates;
        private int interval = 60 * 60;
        private TimeUnit unit = TimeUnit.Seconds;
        private Repeat repeat = Repeat.Indefinitely;

        protected ScheduleExecutionAttribute()
        {
        }

        public ScheduleExecutionAttribute(Repeat repeat)
        {
            this.repeat = repeat;
        }

        public ScheduleExecutionAttribute(int seconds)
        {
            interval = seconds;
        }

        public ScheduleExecutionAttribute(int interval, TimeUnit unit)
        {
            this.interval = interval;
            this.unit = unit;
        }

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public TimeUnit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public Repeat Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }

        public Type Decorates
        {
            get { return decorates; }
            set { decorates = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

        public bool IsAuthorized(System.Security.Principal.IPrincipal user)
        {
            return true;
        }

        #region IComparable<IPlugin> Members

        public int CompareTo(IPlugin other)
        {
            if (other == null)
                return 1;
            int result = SortOrder.CompareTo(other.SortOrder) * 2 + Name.CompareTo(other.Name);
            return result;
        }

        #region Equals & GetHashCode Methods

        public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().IsAssignableFrom(GetType()))
                return false;
            return Name == ((ScheduleExecutionAttribute)obj).Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #endregion
    }
}
