using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using Nop.Services.Topics;

namespace Nop.Web.Infrastructure.Installations;

public partial class KungFuTopicSeeder
{
    private const string TopicFolderVirtualPath = "~/Themes/KungFu/Content/topics";

    private readonly INopFileProvider _fileProvider;
    private readonly ILogger<KungFuTopicSeeder> _logger;
    private readonly ITopicService _topicService;
    private readonly ITopicTemplateService _topicTemplateService;

    public KungFuTopicSeeder(
        INopFileProvider fileProvider,
        ILogger<KungFuTopicSeeder> logger,
        ITopicService topicService,
        ITopicTemplateService topicTemplateService)
    {
        _fileProvider = fileProvider;
        _logger = logger;
        _topicService = topicService;
        _topicTemplateService = topicTemplateService;
    }

    public async Task SeedAsync()
    {
        var templateId = await GetTemplateIdAsync();

        foreach (var descriptor in GetTopicDescriptors())
        {
            var body = await LoadTopicBodyAsync(descriptor.FileName);
            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogWarning("Skipped topic {SystemName} because {FileName} could not be read.", descriptor.SystemName, descriptor.FileName);
                continue;
            }

            await UpsertTopicAsync(descriptor, body, templateId);
        }
    }

    private async Task UpsertTopicAsync(TopicSeedDescriptor descriptor, string body, int templateId)
    {
        var topic = await _topicService.GetTopicBySystemNameAsync(descriptor.SystemName) ?? new Topic
        {
            SystemName = descriptor.SystemName
        };

        topic.IncludeInSitemap = descriptor.IncludeInSitemap;
        topic.DisplayOrder = descriptor.DisplayOrder;
        topic.AccessibleWhenStoreClosed = false;
        topic.IsPasswordProtected = false;
        topic.Password = null;
        topic.Title = descriptor.Title;
        topic.Body = body;
        topic.Published = true;
        topic.TopicTemplateId = templateId;
        topic.MetaKeywords = null;
        topic.MetaDescription = null;
        topic.MetaTitle = descriptor.Title;
        topic.SubjectToAcl = false;
        topic.LimitedToStores = false;
        topic.AvailableStartDateTimeUtc = null;
        topic.AvailableEndDateTimeUtc = null;

        if (topic.Id == 0)
            await _topicService.InsertTopicAsync(topic);
        else
            await _topicService.UpdateTopicAsync(topic);
    }

    private async Task<int> GetTemplateIdAsync()
    {
        var templates = await _topicTemplateService.GetAllTopicTemplatesAsync();
        var defaultTemplate = templates
            ?.OrderBy(t => t.DisplayOrder)
            .FirstOrDefault();

        return defaultTemplate?.Id ?? 1;
    }

    private async Task<string> LoadTopicBodyAsync(string fileName)
    {
        var topicsDirectory = _fileProvider.MapPath(TopicFolderVirtualPath);
        var filePath = _fileProvider.Combine(topicsDirectory, fileName);

        if (!_fileProvider.FileExists(filePath))
            return null;

        return await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
    }

    private static IEnumerable<TopicSeedDescriptor> GetTopicDescriptors()
    {
        return new List<TopicSeedDescriptor>
        {
            new("AboutUs", "About us", 20, "about-us.html"),
            new("CheckoutAsGuestOrRegister", "Checkout as guest or register", 1, "checkout-as-guest-or-register.html"),
            new("ConditionsOfUse", "Conditions of use", 15, "conditions-of-use.html"),
            new("ContactUs", "Contact us", 1, "contact-us.html"),
            new("ForumWelcomeMessage", "Forums", 1, "forum-welcome-message.html"),
            new("HomepageText", "Online store for serious students", 1, "homepage-text.html"),
            new("LoginRegistrationInfo", "About login / registration", 1, "login-registration-info.html"),
            new("PrivacyInfo", "Privacy notice", 10, "privacy-info.html"),
            new("PageNotFound", "Page not found", 1, "page-not-found.html"),
            new("ShippingInfo", "Shipping & returns", 5, "shipping-info.html"),
            new("ApplyVendor", "Apply as a vendor", 1, "apply-vendor.html"),
            new("VendorTermsOfService", "Terms of service for vendors", 1, "vendor-terms-of-service.html")
        };
    }

    private record TopicSeedDescriptor(string SystemName, string Title, int DisplayOrder, string FileName, bool IncludeInSitemap = false);
}
