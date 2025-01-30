﻿using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Topics;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class TopicController : BasePublicController
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITopicModelFactory _topicModelFactory;
    protected readonly ITopicService _topicService;

    #endregion

    #region Ctor

    public TopicController(IAclService aclService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IStoreMappingService storeMappingService,
        ITopicModelFactory topicModelFactory,
        ITopicService topicService)
    {
        _aclService = aclService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _storeMappingService = storeMappingService;
        _topicModelFactory = topicModelFactory;
        _topicService = topicService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> TopicDetails(int topicId)
    {
        var topic = await _topicService.GetTopicByIdAsync(topicId);

        if (topic == null)
            return InvokeHttp404();

        var notAvailable = !topic.Published ||
                           //availability dates
                           !_topicService.TopicIsAvailable(topic) ||
                           //ACL (access control list)
                           !await _aclService.AuthorizeAsync(topic) ||
                           //store mapping
                           !await _storeMappingService.AuthorizeAsync(topic);

        //allow administrators to preview any topic
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.ContentManagement.TOPICS_VIEW);

        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        var model = await _topicModelFactory.PrepareTopicModelAsync(topic);

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.ADMIN }));

        //template
        var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
        return View(templateViewPath, model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> TopicDetailsPopup(string systemName)
    {
        var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
        if (model == null)
            return InvokeHttp404();

        ViewBag.IsPopup = true;

        //template
        var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
        return PartialView(templateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Authenticate(int id, string password)
    {
        var authResult = new AuthenticatedTopicModel();
        var topic = await _topicService.GetTopicByIdAsync(id);

        if (topic == null ||
            !topic.Published ||
            !_topicService.TopicIsAvailable(topic) ||
            !await _storeMappingService.AuthorizeAsync(topic) ||
            !await _aclService.AuthorizeAsync(topic))
        {
            return Json(authResult with { Error = "Topic is not found" });
        }

        if (topic.IsPasswordProtected)
        {
            if (topic.Password != null && topic.Password.Equals(password))
            {
                return Json(authResult with { 
                    Authenticated = true, 
                    Title = await _localizationService.GetLocalizedAsync(topic, x => x.Title),
                    Body = await _localizationService.GetLocalizedAsync(topic, x => x.Body)
                });
            }
            else
            {
                return Json(authResult with { Error = await _localizationService.GetResourceAsync("Topic.WrongPassword") });
            }
        }

        return Json(authResult);
    }

    #endregion
}