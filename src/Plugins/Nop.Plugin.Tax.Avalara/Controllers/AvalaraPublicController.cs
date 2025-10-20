﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.Tax.Avalara.Models.Customer;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Customers;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraPublicController : BasePublicController
{
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AvalaraPublicController(AvalaraTaxManager avalaraTaxManager,
        AvalaraTaxSettings avalaraTaxSettings,
        ICustomerService customerService,
        ITaxPluginManager taxPluginManager,
        IWorkContext workContext)
    {
        _avalaraTaxManager = avalaraTaxManager;
        _avalaraTaxSettings = avalaraTaxSettings;
        _customerService = customerService;
        _taxPluginManager = taxPluginManager;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> ExemptionCertificates()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!_avalaraTaxSettings.EnableCertificates)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //ACL
        if (_avalaraTaxSettings.CustomerRoleIds.Any())
        {
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
        }

        var token = await _avalaraTaxManager.CreateTokenAsync(customer);
        var link = await _avalaraTaxManager.GetInvitationAsync(customer) ?? AvalaraTaxDefaults.CertExpressUrl;
        var certificates = await _avalaraTaxManager.GetCustomerCertificatesAsync(customer);
        var model = new TaxExemptionModel
        {
            Token = token,
            Link = link,
            CustomerId = customer.Id,
            Certificates = certificates?.Select(certificate => new ExemptionCertificateModel
            {
                Id = certificate.id ?? 0,
                Status = certificate.status,
                SignedDate = certificate.signedDate.ToShortDateString(),
                ExpirationDate = certificate.expirationDate.ToShortDateString(),
                ExposureZone = certificate.exposureZone?.name
            }).ToList() ?? new List<ExemptionCertificateModel>(),
            AvailableExposureZones = (await _avalaraTaxManager.GetExposureZonesAsync())
                .Select(zone => new SelectListItem(zone.name, zone.name))
                .ToList()
        };

        return View("~/Plugins/Tax.Avalara/Views/Customer/ExemptionCertificates.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> DownloadCertificate(int id)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!_avalaraTaxSettings.EnableCertificates)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //ACL
        if (_avalaraTaxSettings.CustomerRoleIds.Any())
        {
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
        }

        //try to get a file by the identifier
        var file = await _avalaraTaxManager.DownloadCertificateAsync(id);
        if (file is null)
            return InvokeHttp404();

        return File(file.Data, file.ContentType, file.Filename?.Split(';')?.FirstOrDefault() ?? "certificate");
    }

    #endregion
}