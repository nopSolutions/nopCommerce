using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Theme.KungFu;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Topics;

namespace Nop.Plugin.Theme.KungFu.Services;

public class ThemeKungFuService : IThemeKungFuService
{
    private readonly ILogger<ThemeKungFuService> _logger;
    private readonly INopFileProvider _fileProvider;
    private readonly ISettingService _settingService;
    private readonly ITopicService _topicService;
    private readonly ITopicTemplateService _topicTemplateService;
    private readonly IPluginService _pluginService;
    private readonly IMessageTemplateService _messageTemplateService;
    private readonly IRepository<EmailAccount> _emailAccountRepository;

    public ThemeKungFuService(
        ILogger<ThemeKungFuService> logger,
        INopFileProvider fileProvider,
        ISettingService settingService,
        ITopicService topicService,
        ITopicTemplateService topicTemplateService,
        IPluginService pluginService,
        IMessageTemplateService messageTemplateService,
        IRepository<EmailAccount> emailAccountRepository)
    {
        _logger = logger;
        _fileProvider = fileProvider;
        _settingService = settingService;
        _topicService = topicService;
        _topicTemplateService = topicTemplateService;
        _pluginService = pluginService;
        _messageTemplateService = messageTemplateService;
        _emailAccountRepository = emailAccountRepository;
    }

    public async Task<ThemeSyncResult> EnsureSyncedAsync(bool force = false)
    {
        var settings = await _settingService.LoadSettingAsync<ThemeKungFuSettings>();
        var descriptor = await GetDescriptorAsync();
        var pluginVersion = descriptor?.Version ?? "";
        var outdated = !string.Equals(settings.LastSyncedVersion, pluginVersion, StringComparison.OrdinalIgnoreCase);
        var shouldSync = force || outdated || (settings.SyncAutomatically && !settings.LastSyncedOnUtc.HasValue);

        var result = new ThemeSyncResult
        {
            WasOutdated = outdated,
            SyncedOnUtc = settings.LastSyncedOnUtc,
            PluginVersion = pluginVersion,
            Synced = false
        };

        if (!shouldSync)
            return result;

        await CopyThemeAsync(descriptor);
        await SeedTopicsAsync();
        await SeedMessageTemplatesAsync();

        settings.LastSyncedOnUtc = DateTime.UtcNow;
        settings.LastSyncedVersion = pluginVersion;
        await _settingService.SaveSettingAsync(settings);

        result.SyncedOnUtc = settings.LastSyncedOnUtc;
        result.Synced = true;
        result.WasOutdated = outdated;

        return result;
    }

    public async Task<ThemeSyncResult> GetStatusAsync()
    {
        var settings = await _settingService.LoadSettingAsync<ThemeKungFuSettings>();
        var descriptor = await GetDescriptorAsync();
        var pluginVersion = descriptor?.Version ?? "";

        return new ThemeSyncResult
        {
            PluginVersion = pluginVersion,
            SyncedOnUtc = settings.LastSyncedOnUtc,
            WasOutdated = !string.Equals(settings.LastSyncedVersion, pluginVersion, StringComparison.OrdinalIgnoreCase),
            Synced = false
        };
    }

    private async Task<PluginDescriptor> GetDescriptorAsync()
    {
        return await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(ThemeKungFuDefaults.SystemName, LoadPluginsMode.All);
    }

    private async Task CopyThemeAsync(PluginDescriptor descriptor)
    {
        var pluginDirectory = descriptor is null
            ? _fileProvider.MapPath("~/Plugins/" + ThemeKungFuDefaults.SystemName)
            : _fileProvider.GetDirectoryName(descriptor.OriginalAssemblyFile);
        var sourceDirectory = _fileProvider.Combine(pluginDirectory, "Themes", ThemeKungFuDefaults.ThemeName);
        var themesDirectory = _fileProvider.MapPath(NopPluginDefaults.ThemesPath);
        var destinationDirectory = _fileProvider.Combine(themesDirectory, ThemeKungFuDefaults.ThemeName);

        if (!_fileProvider.DirectoryExists(sourceDirectory))
        {
            _logger.LogWarning("Kung Fu theme source directory not found at {SourceDirectory}", sourceDirectory);
            return;
        }

        if (_fileProvider.DirectoryExists(destinationDirectory))
            _fileProvider.DeleteDirectory(destinationDirectory);

        _fileProvider.CreateDirectory(destinationDirectory);
        await CopyDirectoryAsync(sourceDirectory, destinationDirectory);
    }

    private async Task CopyDirectoryAsync(string source, string destination)
    {
        foreach (var file in _fileProvider.EnumerateFiles(source, "*", true))
        {
            var targetPath = _fileProvider.Combine(destination, _fileProvider.GetFileName(file));
            _fileProvider.FileCopy(file, targetPath, true);
        }

        foreach (var directory in _fileProvider.GetDirectories(source, topDirectoryOnly: true))
        {
            var directoryName = _fileProvider.GetDirectoryNameOnly(directory);
            var targetSubDirectory = _fileProvider.Combine(destination, directoryName);
            _fileProvider.CreateDirectory(targetSubDirectory);
            await CopyDirectoryAsync(directory, targetSubDirectory);
        }
    }

    private async Task SeedTopicsAsync()
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

    private async Task<int> GetTemplateIdAsync()
    {
        var templates = await _topicTemplateService.GetAllTopicTemplatesAsync();
        var defaultTemplate = templates?.OrderBy(t => t.DisplayOrder).FirstOrDefault();

        return defaultTemplate?.Id ?? 1;
    }

    private async Task<string> LoadTopicBodyAsync(string fileName)
    {
        var topicsDirectory = _fileProvider.MapPath(ThemeKungFuDefaults.TopicsVirtualPath);
        var filePath = _fileProvider.Combine(topicsDirectory, fileName);

        if (!_fileProvider.FileExists(filePath))
            return null;

        return await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
    }

    private async Task UpsertTopicAsync(TopicSeedDescriptor descriptor, string body, int templateId)
    {
        var topic = await _topicService.GetTopicBySystemNameAsync(descriptor.SystemName) ?? new Core.Domain.Topics.Topic
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

    private async Task SeedMessageTemplatesAsync()
    {
        var emailAccount = await _emailAccountRepository.Table.FirstOrDefaultAsync();
        if (emailAccount == null)
        {
            _logger.LogWarning("No email account found. Skipping message template sync.");
            return;
        }

        foreach (var descriptor in GetMessageTemplateDescriptors())
        {
            var body = await LoadMessageTemplateBodyAsync(descriptor.FileName);
            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogWarning("Skipped message template {SystemName} because {FileName} could not be read.", descriptor.SystemName, descriptor.FileName);
                continue;
            }

            await UpsertMessageTemplateAsync(descriptor, body, emailAccount.Id);
        }
    }

    private async Task<string> LoadMessageTemplateBodyAsync(string fileName)
    {
        var templatesDirectory = _fileProvider.MapPath(ThemeKungFuDefaults.MessageTemplatesVirtualPath);
        var filePath = _fileProvider.Combine(templatesDirectory, fileName);

        if (!_fileProvider.FileExists(filePath))
            return null;

        return await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
    }

    private async Task UpsertMessageTemplateAsync(MessageTemplateSeedDescriptor descriptor, string body, int emailAccountId)
    {
        var templates = await _messageTemplateService.GetMessageTemplatesByNameAsync(descriptor.SystemName);
        var template = templates.FirstOrDefault() ?? new MessageTemplate
        {
            Name = descriptor.SystemName
        };

        template.Subject = descriptor.Subject;
        template.Body = body;
        template.IsActive = descriptor.IsActive;
        template.EmailAccountId = emailAccountId;

        if (template.Id == 0)
            await _messageTemplateService.InsertMessageTemplateAsync(template);
        else
            await _messageTemplateService.UpdateMessageTemplateAsync(template);
    }

    private static IEnumerable<MessageTemplateSeedDescriptor> GetMessageTemplateDescriptors()
    {
        return new List<MessageTemplateSeedDescriptor>
        {
            new(MessageTemplateSystemNames.ORDER_PLACED_CUSTOMER_NOTIFICATION, 
                "Order receipt from %Store.Name%", 
                "order-placed-customer.html", 
                true),
            new(MessageTemplateSystemNames.ORDER_COMPLETED_CUSTOMER_NOTIFICATION, 
                "%Store.Name% - Your order completed", 
                "order-completed-customer.html", 
                true),
            new(MessageTemplateSystemNames.CUSTOMER_WELCOME_MESSAGE, 
                "Welcome to %Store.Name% - Begin Your Journey", 
                "customer-welcome.html", 
                true),
            new(MessageTemplateSystemNames.SHIPMENT_SENT_CUSTOMER_NOTIFICATION, 
                "Your order from %Store.Name% has been %if (!%Order.IsCompletelyShipped%) partially endif%shipped", 
                "shipment-sent-customer.html", 
                true),
            new(MessageTemplateSystemNames.CUSTOMER_PASSWORD_RECOVERY_MESSAGE, 
                "%Store.Name% - Password Recovery", 
                "password-recovery.html", 
                true),
            new(MessageTemplateSystemNames.ORDER_PAID_AI_SAGE_NOTIFICATION, 
                "%Store.Name% - Wisdom from the Sage", 
                "order-paid-ai-sage.html", 
                false) // Disabled by default until AI is configured
        };
    }

    private record TopicSeedDescriptor(string SystemName, string Title, int DisplayOrder, string FileName, bool IncludeInSitemap = false);
    private record MessageTemplateSeedDescriptor(string SystemName, string Subject, string FileName, bool IsActive);
}
