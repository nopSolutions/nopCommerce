using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Factories
{
    /// <summary>
    /// Represents the base localized model factory implementation
    /// </summary>
    public partial class LocalizedModelFactory : ILocalizedModelFactory
    {
        #region Fields
        
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public LocalizedModelFactory(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare localized model for localizable entities
        /// </summary>
        /// <typeparam name="T">Localized model type</typeparam>
        /// <param name="configure">Model configuration action</param>
        /// <returns>List of localized model</returns>
        public virtual IList<T> PrepareLocalizedModels<T>(Action<T, int> configure = null) where T : ILocalizedLocaleModel
        {
            //get all available languages
            var availableLanguages = _languageService.GetAllLanguages(true);

            //prepare models
            var localizedModels = availableLanguages.Select(language =>
            {
                //create localized model
                var localizedModel = Activator.CreateInstance<T>();

                //set language
                localizedModel.LanguageId = language.Id;

                //invoke the model configuration action
                configure?.Invoke(localizedModel, localizedModel.LanguageId);

                return localizedModel;
            }).ToList();

            return localizedModels;
        }

        #endregion
    }
}