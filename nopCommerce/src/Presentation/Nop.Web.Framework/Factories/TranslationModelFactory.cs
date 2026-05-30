using System.Reflection;
using DeepL;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Translation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents translation model factory implementation
/// </summary>
public partial class TranslationModelFactory : ITranslationModelFactory
{
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected TranslationClientHelper _translationClientHelper;
    protected readonly TranslationSettings _translationSettings;

    #endregion

    #region Ctor

    public TranslationModelFactory(ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        TranslationSettings translationSettings)
    {
        _languageService = languageService;
        _localizationService = localizationService;
        _logger = logger;
        _translationClientHelper = null;
        _translationSettings = translationSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare translation model by the passed localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation model
    /// </returns>
    public virtual async Task<TranslationModel> PrepareTranslationModelAsync<T>(ILocalizedModel<T> model,
        params string[] propertiesToTranslate)
        where T : ILocalizedLocaleModel
    {
        return await PrepareTranslationModelAsync(model, propertiesToTranslate.Select(p => (p, false)).ToArray());
    }

    /// <summary>
    /// Prepare translation model by the passed localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation model
    /// </returns>
    public virtual async Task<TranslationModel> PrepareTranslationModelAsync<T>(ILocalizedModel<T> model,
        params (string PropertyName, bool IsHtml)[] propertiesToTranslate)
        where T : ILocalizedLocaleModel
    {
        _translationClientHelper ??= new TranslationClientHelper(_translationSettings);

        var result = new TranslationModel();

        var properties = propertiesToTranslate.Select(p => new KeyValuePair<string, bool>(p.PropertyName, p.IsHtml)).ToDictionary();

        //get model properties to use as original text for translation
        var modelProperties = model.GetType().GetProperties()
            .Where(propertyFilter)
            .ToList();

        //get properties to save translated text
        var localizedProperties = typeof(T).GetProperties()
            .Where(propertyFilter)
            .ToList();

        //get original language
        var originalLanguage = await _languageService.GetLanguageByIdAsync(_translationSettings.TranslateFromLanguageId);
        var position = 0;

        //the loop by locales which should be translated
        foreach (var modelLocale in model.Locales)
        {
            position++;

            //we ignore the original language
            if (modelLocale.LanguageId == _translationSettings.TranslateFromLanguageId)
                continue;

            //and languages which should be ignored
            if (_translationSettings.NotTranslateLanguages.Contains(modelLocale.LanguageId))
                continue;

            //get target languages to translation for
            var translateToLanguage = await _languageService.GetLanguageByIdAsync(modelLocale.LanguageId);

            //the loop by properties which should be translated
            foreach (var prop in localizedProperties)
            {
                //get the current value of property
                var currentTranslatedText = prop.GetValue(modelLocale, null)?.ToString();

                //ignore the property which already has a value
                if (!string.IsNullOrEmpty(currentTranslatedText))
                    continue;

                //get original text to translate
                var originText = modelProperties.FirstOrDefault(p => p.Name.Equals(prop.Name))?.GetValue(model, null)?.ToString();

                //ignore the empty original text
                if (string.IsNullOrEmpty(originText))
                    continue;

                try
                {
                    var translatedText = await _translationClientHelper.TranslateAsync(originalLanguage, originText, translateToLanguage, properties[prop.Name]);
                    if (string.IsNullOrEmpty(translatedText))
                        continue;

                    //set translated text to property
                    prop.SetValue(modelLocale, translatedText);
                    var inputName = $"{nameof(model.Locales)}_{position - 1}__{prop.Name}";
                    result.Translations.Add(new()
                    {
                        Name = inputName,
                        Value = translatedText,
                        OriginValue = originText,
                        Language = translateToLanguage.UniqueSeoCode,
                        OriginLanguage = originalLanguage.UniqueSeoCode
                    });
                }
                catch (Exception e)
                {
                    var serviceName = await _localizationService.GetLocalizedEnumAsync((TranslationServiceType)_translationSettings.TranslationServiceId);
                    var errorMessage = $"{serviceName}: {e.Message}";
                    await _logger.ErrorAsync(errorMessage, e);
                    result.HasErrors = true;

                    //DeepL: stop translate if one of the languages aren't support
                    //to reduce error count
                    if (e.Message.Contains("Value for 'target_lang' not supported", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("Value for 'source_lang' not supported", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
            }
        }

        return result;

        //filter for get only string property which should be translated
        bool propertyFilter(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(string) && properties.ContainsKey(propertyInfo.Name);
        }
    }

    #endregion

    #region Nested class

    /// <summary>
    /// Helper class to translation client
    /// </summary>
    protected class TranslationClientHelper
    {
        #region Fields

        protected readonly Google.Cloud.Translation.V2.TranslationClient _googleClient;
        protected readonly DeepLClient _deeplClient;

        #endregion

        #region Ctor

        public TranslationClientHelper(TranslationSettings translationSettings)
        {
            switch ((TranslationServiceType)translationSettings.TranslationServiceId)
            {
                case TranslationServiceType.GoogleTranslate:
                    {
                        if (!string.IsNullOrEmpty(translationSettings.GoogleApiKey))
                            _googleClient = Google.Cloud.Translation.V2.TranslationClient.CreateFromApiKey(translationSettings.GoogleApiKey);
                        _deeplClient = null;
                    }
                    break;

                case TranslationServiceType.DeepL:
                    {
                        if (!string.IsNullOrEmpty(translationSettings.DeepLAuthKey))
                            _deeplClient = new DeepLClient(translationSettings.DeepLAuthKey);
                        _googleClient = null;
                    }
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Translate text or html
        /// </summary>
        /// <param name="originalLanguage">The language to translate from</param>
        /// <param name="originText">The text or HTML to translate</param>
        /// <param name="targetLanguage">The target language to translate</param>
        /// <param name="isHtml">Indicate whether the text to translate should be considered as HTML</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the translated text
        /// </returns>
        public virtual async Task<string> TranslateAsync(Language originalLanguage, string originText, Language targetLanguage, bool isHtml)
        {
            if (_googleClient != null)
            {
                var response = isHtml
                    ? await _googleClient.TranslateHtmlAsync(originText, targetLanguage.UniqueSeoCode, originalLanguage.UniqueSeoCode)
                    : await _googleClient.TranslateTextAsync(originText, targetLanguage.UniqueSeoCode, originalLanguage.UniqueSeoCode);

                return response.TranslatedText;
            }

            if (_deeplClient != null)
            {
                var response = await _deeplClient.TranslateTextAsync(originText, originalLanguage.UniqueSeoCode, targetLanguage.UniqueSeoCode, new TextTranslateOptions
                {
                    TagHandling = isHtml ? "html" : null
                });

                return response.Text;
            }

            return string.Empty;
        }

        #endregion
    }

    #endregion
}