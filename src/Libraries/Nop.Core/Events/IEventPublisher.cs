using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T eventMessage);
    }
}
