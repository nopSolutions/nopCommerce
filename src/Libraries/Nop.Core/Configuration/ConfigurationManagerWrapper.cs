using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// A testable configuration manager wrapper.
    /// </summary>
    public class ConfigurationManagerWrapper
    {
        readonly string sectionGroup;

        public ConfigurationManagerWrapper()
            : this("nop")
        {
        }
        public ConfigurationManagerWrapper(string sectionGroup)
        {
            this.sectionGroup = sectionGroup;
        }

        public ContentSectionTable Sections { get; protected set; }

        public virtual T GetSection<T>(string sectionName) where T : ConfigurationSection
        {
            object section = ConfigurationManager.GetSection(sectionName);
            if (section == null) throw new ConfigurationErrorsException("Missing configuration section at '" + sectionName + "'");
            T contentSection = section as T;
            if (contentSection == null) throw new ConfigurationErrorsException("The configuration section at '" + sectionName + "' is of type '" + section.GetType().FullName + "' instead of '" + typeof(T).FullName + "' which is required.");
            return contentSection;
        }

        public virtual T GetContentSection<T>(string relativeSectionName) where T : ConfigurationSectionBase
        {
            return GetSection<T>(sectionGroup + "/" + relativeSectionName);
        }

        public virtual ConnectionStringsSection GetConnectionStringsSection()
        {
            return GetSection<ConnectionStringsSection>("connectionStrings");
        }

        public virtual string GetConnectionString()
        {
            //TODO:Get connection string here?
            throw new NotImplementedException();
        }


        /// <summary>
        /// Keeps references to used config sections.
        /// </summary>
        public class ContentSectionTable
        {
            public ContentSectionTable(EngineSection engine, HostSection host)
            {
                Engine = engine;
                Host = host;
            }

            public EngineSection Engine { get; protected set; }

            public HostSection Host { get; protected set; }
        }

        #region IAutoStart Members

        public void Start()
        {
            Sections = new ContentSectionTable(GetContentSection<EngineSection>("engine"), 
                GetContentSection<HostSection>("host"));
        }

        public void Stop()
        {
            Sections = null;
        }

        #endregion
    }
}
