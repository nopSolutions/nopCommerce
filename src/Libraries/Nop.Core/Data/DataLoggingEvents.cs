using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Data
{
    public static class DataLoggingEvents
    {
#pragma warning disable IDE1006 // Naming Styles
        internal const int DataConnectionStringEmpty = 10000;

        internal const int DataConnectionStringWellFormatted = 10001;

        internal const int DataConnectionOpenned = 10002;

        internal const int DataConnectionReaderOpenned = 10003;

        internal const int DataConnectionTablesIntersect = 10004;

        internal const int DataConnection = 10005;
#pragma warning restore IDE1006 // Naming Styles
    }
}
