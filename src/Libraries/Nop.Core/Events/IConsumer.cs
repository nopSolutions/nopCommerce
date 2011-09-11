using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Events
{
    public interface IConsumer<T>
    {
        void Handle(T eventMessage);
    }
}
