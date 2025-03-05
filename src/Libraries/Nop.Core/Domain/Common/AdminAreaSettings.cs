﻿using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common;

/// <summary>
/// Admin area settings
/// </summary>
public partial class AdminAreaSettings : ISettings
{
    /// <summary>
    /// Default grid page size
    /// </summary>
    public int DefaultGridPageSize { get; set; }

    /// <summary>
    /// Products bulk edit grid page size
    /// </summary>
    public int ProductsBulkEditGridPageSize { get; set; }

    /// <summary>
    /// Popup grid page size (for popup pages)
    /// </summary>
    public int PopupGridPageSize { get; set; }

    /// <summary>
    /// A comma-separated list of available grid page sizes
    /// </summary>
    public string GridPageSizes { get; set; }

    /// <summary>
    /// Additional settings for rich editor
    /// </summary>
    public string RichEditorAdditionalSettings { get; set; }

    /// <summary>
    /// A value indicating whether to javascript is supported in rich editor
    /// </summary>
    public bool RichEditorAllowJavaScript { get; set; }

    /// <summary>
    /// A value indicating whether to style tag is supported in rich editor
    /// </summary>
    public bool RichEditorAllowStyleTag { get; set; }

    /// <summary>
    /// A value indicating whether to use rich text editor on email messages for customers
    /// </summary>
    public bool UseRichEditorForCustomerEmails { get; set; }

    /// <summary>
    /// A value indicating whether to use rich editor on message templates and campaigns details pages
    /// </summary>
    public bool UseRichEditorInMessageTemplates { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether advertisements (news) should be hidden
    /// </summary>
    public bool HideAdvertisementsOnAdminArea { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should check the store for license compliance
    /// </summary>
    public bool CheckLicense { get; set; }

    /// <summary>
    /// Gets or sets title of last news (admin area)
    /// </summary>
    public string LastNewsTitleAdminArea { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use IsoDateFormat in JSON results (used for avoiding issue with dates in grids)
    /// </summary>
    public bool UseIsoDateFormatInJsonResult { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to documantation reference links on pages
    /// </summary>
    public bool ShowDocumentationReferenceLinks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to the content header should be sticky when scrolling
    /// </summary>
    public bool UseStickyHeaderLayout { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of drop-down list items to display search input.
    /// </summary>
    public int MinimumDropdownItemsForSearch { get; set; }
}