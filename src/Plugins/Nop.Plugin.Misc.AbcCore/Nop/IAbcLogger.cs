using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public interface IAbcLogger : ILogger
    {
        IList<Log> GetPageNotFoundLogs();
    }
}
