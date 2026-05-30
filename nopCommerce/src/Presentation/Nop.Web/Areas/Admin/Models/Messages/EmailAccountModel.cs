using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Messages;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents an email account model
/// </summary>
public partial record EmailAccountModel : BaseNopEntityModel
{
    #region Properties

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Email")]
    public string Email { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.DisplayName")]
    public string DisplayName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Host")]
    public string Host { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Port")]
    public int Port { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Username")]
    public string Username { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Password")]
    [NoTrim]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EnableSsl")]
    public bool EnableSsl { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.IsDefaultEmailAccount")]
    public bool IsDefaultEmailAccount { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo")]
    public string SendTestEmailTo { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.MaxNumberOfEmails")]
    public int MaxNumberOfEmails { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EmailAuthenticationMethod")]
    public EmailAuthenticationMethod EmailAuthenticationMethod { get; set; }
    public List<SelectListItem> AvailableEmailAuthenticationMethods { get; set; } = new();

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.ClientId")]
    public string ClientId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.ClientSecret")]
    [NoTrim]
    [DataType(DataType.Password)]
    public string ClientSecret { get; set; }

    [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.TenantId")]
    public string TenantId { get; set; }

    public string AuthUrl { get; set; }

    #endregion
}