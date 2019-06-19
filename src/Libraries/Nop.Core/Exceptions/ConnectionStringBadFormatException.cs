using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Exceptions
{
    public class ConnectionStringBadFormatException : NopException
    {
        public ConnectionStringBadFormatException(string connectionString)
            : base($"Bad format connection string. conn => {connectionString}")
        {
        }
    }
}
