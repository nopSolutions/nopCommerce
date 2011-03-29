using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class LogController : BaseNopController
    {
        private ILogger _logger;

        public LogController(ILogger logger)
        {
            _logger = logger;
        }

        public ActionResult List()
        {
            var logs = _logger.GetAllLogs(null, null, string.Empty, null, 0, 10);
            var model = new GridModel<LogModel>
            {
                Data = logs.Select(x => x.ToModel()),
                Total = logs.TotalCount
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var logs = _logger.GetAllLogs(null, null, string.Empty, null, command.Page - 1, command.PageSize);
            var model = new GridModel<LogModel>
            {
                Data = logs.Select(x => x.ToModel()),
                Total = logs.TotalCount
            };
            return new JsonResult
            {
                Data = model
            };
        }
    }
}
