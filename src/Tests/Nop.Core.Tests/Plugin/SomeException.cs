using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Tests.Plugin
{
    public class SomeException : Exception
    {
        public SomeException(string message)
            : base(message)
        {
        }
    }
}
