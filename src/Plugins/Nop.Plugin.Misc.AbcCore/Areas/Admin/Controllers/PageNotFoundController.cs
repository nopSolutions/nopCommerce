using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Models.Extensions;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Services.Seo;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Areas.Admin.Models;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Services.Logging;
using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Services.Helpers;
using System;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Controllers
{
    public class PageNotFoundController : BaseAdminController
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IAbcLogger _logger;

        public PageNotFoundController(
            IDateTimeHelper dateTimeHelper,
            IAbcLogger logger)
        {
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public IActionResult List()
        {
            return View(
                "~/Plugins/Misc.AbcCore/Areas/Admin/Views/PageNotFound/List.cshtml",
                new PageNotFoundSearchModel()
            );
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PageNotFoundSearchModel searchModel)
        {
            var logs = _logger.GetPageNotFoundLogs();
            if (!string.IsNullOrWhiteSpace(searchModel.Slug))
            {
                logs = logs.Where(logs => logs.PageUrl.Contains(searchModel.Slug)).ToList();
            }
            var pagedList = logs.ToPagedList(searchModel);
            var model = await new PageNotFoundListModel().PrepareToGridAsync(searchModel, pagedList, () =>
            {
                //fill in model values from the entity
                return pagedList.SelectAwait(async log =>
                {
                    var PageNotFoundModel = new PageNotFoundModel()
                    {
                        Slug = log.PageUrl,
                        ReferrerUrl = log.ReferrerUrl,
                        Date = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc)
                    };

                    return PageNotFoundModel;
                });
            });

            return Json(model);
        }
    }
}
