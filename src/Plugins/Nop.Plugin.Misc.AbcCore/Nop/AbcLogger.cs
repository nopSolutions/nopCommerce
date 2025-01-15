using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Seo;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Common;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcLogger : DefaultLogger, IAbcLogger
    {
        private readonly IRepository<Log> _logRepository;

        public AbcLogger(CommonSettings commonSettings,
            IRepository<Log> logRepository,
            IWebHelper webHelper) :
            base(commonSettings, logRepository, webHelper)
        {
            _logRepository = logRepository;
        }

        public IList<Log> GetPageNotFoundLogs()
        {
            return _logRepository.Table
                                 .Where(log => log.ShortMessage.Contains("Error 404."))
                                 .OrderByDescending(log => log.CreatedOnUtc)
                                 .ToList();
        }
    }
}