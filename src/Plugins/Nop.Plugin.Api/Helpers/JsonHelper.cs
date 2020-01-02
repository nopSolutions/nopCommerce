using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Helpers
{
    public class JsonHelper : IJsonHelper
    {

        #region Private Fields

        private readonly ILocalizationService _localizationService;

        private readonly int _languageId;

        #endregion

        #region Constructors

        public JsonHelper(ILanguageService languageService, ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            var language = languageService.GetAllLanguages().FirstOrDefault();
            _languageId = language != null ? language.Id : 0;
        }

        #endregion

        #region Public Methods

        public Dictionary<string, object> GetRequestJsonDictionaryFromStream(Stream stream, bool rewindStream)
        {
            var json = GetRequestBodyString(stream, rewindStream);
            if (string.IsNullOrEmpty(json))
            {
                throw new InvalidOperationException(_localizationService.GetResource("Api.NoJsonProvided", _languageId, false));
            }

            var requestBodyDictioanry = DeserializeToDictionary(json);
            if (requestBodyDictioanry == null || requestBodyDictioanry.Count == 0)
            {
                throw new InvalidOperationException(_localizationService.GetResource("Api.InvalidJsonFormat", _languageId, false));
            }

            return requestBodyDictioanry;
        }

        public string GetRootPropertyName<T>() where T : class, new()
        {
            var rootProperty = "";

            var jsonObjectAttribute = ReflectionHelper.GetJsonObjectAttribute(typeof(T));
            if (jsonObjectAttribute != null)
            {
                rootProperty = jsonObjectAttribute.Title;
            }

            if (string.IsNullOrEmpty(rootProperty))
            {
                throw new InvalidOperationException($"Error getting root property for type {typeof(T).FullName}.");
            }

            return rootProperty;
        }

        #endregion

        #region Private Methods

        // source - http://stackoverflow.com/questions/5546142/how-do-i-use-json-net-to-deserialize-into-nested-recursive-dictionary-and-list
        private Dictionary<string, object> DeserializeToDictionary(string json)
        {
            //TODO: JToken.Parse could throw an exeption if not valid JSON string is passed
            try
            {
                return ToObject(JToken.Parse(json)) as Dictionary<string, object>;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // source - http://stackoverflow.com/questions/5546142/how-do-i-use-json-net-to-deserialize-into-nested-recursive-dictionary-and-list
        private object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }

        private string GetRequestBodyString(Stream stream, bool rewindStream)
        {
            var result = "";

            using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, rewindStream))
            {
                result = streamReader.ReadToEnd();
                if (rewindStream)
                {
                    stream.Position = 0; //reset position to allow reading again later
                }
            }

            return result;
        }

        #endregion

    }
}