using Newtonsoft.Json;

namespace Nop.Plugin.Payments.AmazonPay.Domain;

/// <summary>
/// Represents request error message
/// </summary>
public class RequestErrorMessage
{
    public static RequestErrorMessage Create(string rawData)
    {
        try
        {
            return JsonConvert.DeserializeObject<RequestErrorMessage>(rawData);
        }
        catch
        {
            //ignore
        }

        return new RequestErrorMessage { Message = $"{AmazonPayDefaults.PluginSystemName}: No response from service" };
    }

    #region Properties

    [JsonProperty("reasonCode")]
    public string ReasonCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    #endregion
}