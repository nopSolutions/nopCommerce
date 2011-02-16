using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Web
{
    public interface IErrorHandler
    {
        void Notify(Exception ex);
    }
}
