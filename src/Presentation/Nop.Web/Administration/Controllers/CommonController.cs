using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Directory;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class CommonController : BaseNopController
	{
		#region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly StoreInformationSettings _storeInformationSettings;

	    #endregion

		#region Constructors

        public CommonController(IDateTimeHelper dateTimeHelper,
            StoreInformationSettings storeInformationSettings)
		{
            this._dateTimeHelper = dateTimeHelper;
            this._storeInformationSettings = storeInformationSettings;
		}

		#endregion Constructors 

        #region Methods

        public ActionResult SystemInfo()
        {
            var model = new SystemInfoModel();
            model.NopVersion = _storeInformationSettings.CurrentVersion;
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
            }
            catch (Exception) { }
            try
            {
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
            }
            catch (Exception) { }
            try
            {
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch (Exception) { }
            model.ServerTimeZone = TimeZone.CurrentTimeZone.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            //Environment.GetEnvironmentVariable("USERNAME");
            return View(model);
        }


        #endregion
    }
}
