using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Text.Json;

namespace AO.ChatGPT
{
    public class Translations
    {
        private string _apiKey;
        private string _gptModel;
        
        public Translations(string apiKey, string gptModel) 
        { 
            _apiKey = apiKey;
            _gptModel = gptModel;
        }

        public async Task<string> TranslateText(string inputLanguage, string outputLanguage, string textToTranslate)
        {
            try
            {
                string endpoint = "https://api.openai.com/v1/chat/completions";
                string decodedText = string.Empty;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", _apiKey);

                    textToTranslate = textToTranslate.Replace("\"", "");
                    textToTranslate = textToTranslate.Replace("\"", "\\\"");
                    textToTranslate = Regex.Replace(textToTranslate, @"\s+", " ").Trim();

                    string encodedText = HttpUtility.HtmlEncode(textToTranslate);

                    string requestBody = $@"
                                        {{
                                            ""model"": ""{_gptModel}"",
                                            ""messages"": [
                                                {{""role"": ""system"", ""content"": ""You are a helpful assistant that translates {inputLanguage} to {outputLanguage}. Please provide a direct translation without any additional phrases, and do not remove html tags."" }},
                                                {{""role"": ""user"", ""content"": ""Translate the following {inputLanguage} text to {outputLanguage} and provide the translation as the answer string only: {encodedText}""}}
                                            ]
                                        }}";

                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(endpoint, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response JSON and extract the translated text
                        var translatedText = ExtractTranslatedText(responseContent);

                        decodedText = HttpUtility.HtmlDecode(translatedText);

                        // Replace line breaks with <br> tags
                        decodedText = decodedText.Replace("\n", "<br />");
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
                return decodedText;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static string ExtractTranslatedText(string responseJson)
        {
            // Parse the JSON response and extract the translated text
            // You can use any JSON parsing library of your choice, such as Newtonsoft.Json
            // Here's a simple example using System.Text.Json:


            var jsonDocument = System.Text.Json.JsonDocument.Parse(responseJson);
            var translatedText = string.Empty;
            if (jsonDocument.RootElement.TryGetProperty("choices", out var choicesElement) && choicesElement.ValueKind == JsonValueKind.Array)
            {
                // Check if the "choices" array has at least one element
                if (choicesElement.GetArrayLength() > 0)
                {
                    var firstChoice = choicesElement[0];

                    // Check if the first choice contains the expected properties
                    if (firstChoice.TryGetProperty("message", out var messageElement) && messageElement.ValueKind == JsonValueKind.Object &&
                        messageElement.TryGetProperty("content", out var contentElement) && contentElement.ValueKind == JsonValueKind.String)
                    {
                        translatedText = contentElement.GetString();
                        // Use the translatedText as needed
                    }
                    else
                    {
                        // Handle the case where the expected properties are not present
                    }
                }
                else
                {
                    // Handle the case where the "choices" array is empty
                }
            }
            else
            {
                // Handle the case where the "choices" array is not present or not of the expected type
            }


            //var translatedText = jsonDocument.RootElement
            //    .GetProperty("choices")[0]
            //    .GetProperty("message")
            //    .GetProperty("content")
            //    .GetString();

            // Remove leading and trailing quotation marks
            translatedText = translatedText.Trim('"');
            translatedText = translatedText.Trim('?');
            translatedText = translatedText.Trim(':');

            return translatedText;
        }
    }
}