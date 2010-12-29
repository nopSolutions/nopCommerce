using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Tasks {
    public interface IStartupTask {
        void Execute();
    }
}
