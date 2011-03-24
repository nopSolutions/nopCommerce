using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core
{
    public class NopEventArgs<T> : EventArgs
    {
        public NopEventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; protected set; }
    }
}
