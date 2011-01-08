using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Tests.Localization
{
    [TestFixture]
    public class LanguageServiceTests : ServiceTest
    {
        ILanguageService _languageService;

        [SetUp]
        public void SetUp()
        {
            var repo1 = new EfRepository<Language>(context);
            var cacheManager = new NopNullCache();
            IWorkContext workContext = null;
            this._languageService = new LanguageService(workContext, cacheManager, repo1);
        }

        protected void RegisterTestData()
        {
            var lang1 = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };

            this._languageService.InsertLanguage(lang1);
        }

        [Test]
        public void Can_get_all_languages()
        {
            RegisterTestData();

            var languages = _languageService.GetAllLanguages(true);
            languages.ShouldNotBeNull();
            (languages.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_insert_and_load_language()
        {
            var lang = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageService.InsertLanguage(lang);
            
            var from = _languageService.GetLanguageById(lang.Id);
            from.Name.ShouldEqual("English");
            from.LanguageCulture.ShouldEqual("en-Us");
            from.FlagImageFileName.ShouldEqual("us.png");
            from.Published.ShouldEqual(true);
            from.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_load_and_delete_language()
        {
            RegisterTestData();

            //load the first language
            var language = _languageService.GetLanguageById(1);
            language.ShouldNotBeNull();

            //delete language
            _languageService.DeleteLanguage(language);

            //load th languages
            var from = _languageService.GetLanguageById(language.Id);
            from.ShouldBeNull();
        }
    }
}
