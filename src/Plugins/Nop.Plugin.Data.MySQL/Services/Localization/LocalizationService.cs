using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.Data.MySQL.Services.Localization
{
    public class LocalizationService : Nop.Services.Localization.LocalizationService
    {
        #region Fields

        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Methods

        public LocalizationService(IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILogger logger,
            IRepository<LocaleStringResource> lsrRepository,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
            : base(dataProvider,
                dbContext,
                eventPublisher,
                languageService,
                localizedEntityService,
                logger,
                lsrRepository,
                settingService,
                cacheManager,
                workContext,
                localizationSettings)
        {
            _lsrRepository = lsrRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xml">XML</param>
        /// <param name="updateExistingResources">A value indicating whether to update existing resources</param>
        public override void ImportResourcesFromXml(Language language, string xml, bool updateExistingResources = true)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (string.IsNullOrEmpty(xml))
                return;

            //SQL 2005 insists that your XML schema encoding be in UTF-16.
            //Otherwise, you'll get "XML parsing: line 1, character XXX, unable to switch the encoding"
            //so let's remove XML declaration
            var inDoc = new XmlDocument();
            inDoc.LoadXml(xml);
            var sb = new StringBuilder();
            using (var xWriter = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                inDoc.Save(xWriter);
                xWriter.Close();
            }

            var outDoc = new XmlDocument();
            outDoc.LoadXml(sb.ToString());

            var xnList = outDoc.SelectNodes("/Language/LocaleResource");
            var resources = new List<LocaleStringResource>();
            foreach (XmlNode xn in xnList)
            {
                resources.Add(new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = xn.Attributes["Name"].InnerText,
                    ResourceValue = xn["Value"].InnerText
                });
            }

            _lsrRepository.Insert(resources);

            //clear cache
            _cacheManager.RemoveByPattern(NopLocalizationDefaults.LocaleStringResourcesPatternCacheKey);
        }

        #endregion
    }
}
