using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;
using Nop.Services.ExportImport;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public interface IAbcExportManager : IExportManager
    {
        Task<byte[]> ExportPageNotFoundRecordsToXlsxAsync(IList<Log> logs);
    }
}
