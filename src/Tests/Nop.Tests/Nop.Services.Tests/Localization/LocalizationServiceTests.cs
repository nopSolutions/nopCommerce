using FluentAssertions;
using Humanizer;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Localization;

[TestFixture]
public class LocalizationServiceTests : ServiceTest
{
    private ILocalizationService _localizationService;
    private IRepository<LocaleStringResource> _lsrRepository;
    private const string PREFIX = "Nop.Tests.Nop.Services.Tests.Localization";
    private Dictionary<string, string> _resources;

    [OneTimeSetUp]
    public void SetUp()
    {
        _localizationService = GetService<ILocalizationService>();
        _lsrRepository = GetService<IRepository<LocaleStringResource>>();

        _resources = new Dictionary<string, string>
        {
            [$"{PREFIX}.Val1"] = "Value1",
            [$"{PREFIX}.Val2"] = "Value2",
            [$"{PREFIX}.Val3"] = "Value3"
        };
    }

    [TearDown]
    public async Task TearDown()
    {
        await _localizationService.DeleteLocaleResourcesAsync(PREFIX);
    }

    protected IQueryable<LocaleStringResource> Filter(Dictionary<string, string> resources, IQueryable<LocaleStringResource> query)
    {
        query = query.Where(p =>
            resources.Keys.Select(k => k.ToLowerInvariant()).Contains(p.ResourceName));

        return query;
    }

    [Test]
    public async Task CanAddOrUpdateLocaleResource()
    {
        var localeStringResources = await _lsrRepository.GetAllAsync(query => Filter(_resources, query));

        localeStringResources.Any().Should().BeFalse();

        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        localeStringResources = await _lsrRepository.GetAllAsync(query => Filter(_resources, query));

        localeStringResources.Any().Should().BeTrue();
        localeStringResources.Count.Should().Be(3);
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldIgnoreKeyCase()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Camelize(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Pascalize(), p => p.Value));

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldNotIgnoreValueCase()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);
        rez.Count(p => p.ResourceValue == p.ResourceValue.ToUpperInvariant()).Should().Be(0);

        rez.Count(p => _resources.ContainsValue(p.ResourceValue)).Should().Be(3);

        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToUpperInvariant()));

        rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);
        rez.Count(p => p.ResourceValue == p.ResourceValue.ToUpperInvariant()).Should().Be(3);
        rez.Count(p => _resources.ContainsValue(p.ResourceValue)).Should().Be(0);
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldSkipAlreadyExistsResources()
    {
        LocaleResourceConsumer.UpdateCount = 0;

        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Camelize(), p => p.Value));
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.Pascalize(), p => p.Value));

        LocaleResourceConsumer.UpdateCount.Should().Be(0);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToUpperInvariant()));

        LocaleResourceConsumer.UpdateCount.Should().Be(3);
    }

    [Test]
    public async Task AddOrUpdateLocaleResourceShouldApdateAllResorcesIfLangIdIsNull()
    {
        var languageService = GetService<ILanguageService>();
        var language = new Language
        {
            Name = "test lang",
            LanguageCulture = "en"
        };
        await languageService.InsertLanguageAsync(language);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources, 1);
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources, language.Id);
        LocaleResourceConsumer.UpdateCount = 0;
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources.ToDictionary(p => p.Key, p => p.Value.ToUpperInvariant()), null);
        await languageService.DeleteLanguageAsync(language);
        LocaleResourceConsumer.UpdateCount.Should().Be(6);
    }

    [Test]
    public async Task CanDeleteLocaleResources()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);

        await _localizationService.DeleteLocaleResourcesAsync(PREFIX);

        rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(0);
    }

    [Test]
    public async Task DeleteLocaleResourcesShuoldIgnoreCase()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(_resources);

        var rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(3);

        await _localizationService.DeleteLocaleResourcesAsync(PREFIX.ToUpperInvariant());

        rez = _lsrRepository.Table
            .Where(p => p.ResourceName.StartsWith(PREFIX, StringComparison.InvariantCultureIgnoreCase)).ToList();

        rez.Count.Should().Be(0);
    }

    public class LocaleResourceConsumer : IConsumer<EntityUpdatedEvent<LocaleStringResource>>
    {
        public static int UpdateCount { get; set; }

        public Task HandleEventAsync(EntityUpdatedEvent<LocaleStringResource> eventMessage)
        {
            UpdateCount += 1;

            return Task.CompletedTask;
        }
    }
}