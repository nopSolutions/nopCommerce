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
        
        protected readonly ILanguageService _languageService;

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of localized model
        /// </returns>
        public virtual async Task<IList<T>> PrepareLocalizedModelsAsync<T>(Func<T, int, Task> configure = null) where T : ILocalizedLocaleModel
        {
            //get all available languages
            var availableLanguages = await _languageService.GetAllLanguagesAsync(true);

            //prepare models
            var localizedModels = await availableLanguages.SelectAwait(async language =>
            {
                //create localized model
                var localizedModel = Activator.CreateInstance<T>();

                //set language
                localizedModel.LanguageId = language.Id;

                //invoke the model configuration action
                if (configure != null)
                    await configure(localizedModel, localizedModel.LanguageId);

                return localizedModel;
            }).ToListAsync();

            return localizedModels;
        }

        #endregion
    }
}