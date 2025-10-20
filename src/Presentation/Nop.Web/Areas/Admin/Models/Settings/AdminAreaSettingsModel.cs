﻿using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents an admin area settings model
/// </summary>
public partial record AdminAreaSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminArea.UseRichEditorInMessageTemplates")]
    public bool UseRichEditorInMessageTemplates { get; set; }
    public bool UseRichEditorInMessageTemplates_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminArea.UseStickyHeaderLayout")]
    public bool UseStickyHeaderLayout { get; set; }

    #endregion
}