using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Installation service using SQL files (fast installation)
    /// </summary>
    public partial class SqlFileInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IDbContext _dbContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="storeRepository">Store repository</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="webHelper">Web helper</param>
        public SqlFileInstallationService(IRepository<Language> languageRepository,
            IRepository<Customer> customerRepository,
            IRepository<Store> storeRepository,
            IDbContext dbContext,
            IWebHelper webHelper)
        {
            this._languageRepository = languageRepository;
            this._customerRepository = customerRepository;
            this._storeRepository = storeRepository;
            this._dbContext = dbContext;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Install locales
        /// </summary>
        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(CommonHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }
        }

        /// <summary>
        /// Update default customer
        /// </summary>
        /// <param name="defaultUserEmail">Email</param>
        /// <param name="defaultUserPassword">Password</param>
        protected virtual void UpdateDefaultCustomer(string defaultUserEmail, string defaultUserPassword)
        {
            var adminUser = _customerRepository.Table.Single(x => x.Email == "admin@yourStore.com");
            if (adminUser == null)
                throw new Exception("Admin user cannot be loaded");

            adminUser.CustomerGuid = Guid.NewGuid();
            adminUser.Email = defaultUserEmail;
            adminUser.Username = defaultUserEmail;
            _customerRepository.Update(adminUser);

            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
        }

        /// <summary>
        /// Update default store URL
        /// </summary>
        protected virtual void UpdateDefaultStoreUrl()
        {
            var store = _storeRepository.Table.FirstOrDefault();
            if (store == null)
                throw new Exception("Default store cannot be loaded");

            store.Url = _webHelper.GetStoreLocation(false);
            _storeRepository.Update(store);
        }

        /// <summary>
        /// Execute SQL file
        /// </summary>
        /// <param name="path">File path</param>
        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            foreach (var stmt in statements)
                _dbContext.ExecuteSqlCommand(stmt);
        }

        /// <summary>
        /// Read next statement from stream
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Result</returns>
        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        /// <param name="installSampleData">A value indicating whether to install sample data</param>
        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            ExecuteSqlFile(CommonHelper.MapPath("~/App_Data/Install/Fast/create_required_data.sql"));
            InstallLocaleResources();
            UpdateDefaultCustomer(defaultUserEmail, defaultUserPassword);
            UpdateDefaultStoreUrl();

            if (installSampleData)
            {
                ExecuteSqlFile(CommonHelper.MapPath("~/App_Data/Install/Fast/create_sample_data.sql"));
            }
        }

        #endregion
    }
}