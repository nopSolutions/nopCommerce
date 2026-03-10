using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class CreateContactRequest : ContactInfoDto, IBatchSupport
{
    public CreateContactRequest(string email, bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus,
        bool sendWelcomeMessage)
    {
        Identifiers = [new Identifier(email, isSubscriptionActive, createdOnUtc, inactiveStatus, sendWelcomeMessage) { Type = "email" }];
        SendWelcomeEmail = sendWelcomeMessage;
    }

    [JsonProperty("sendWelcomeEmail")] public bool SendWelcomeEmail { get; }
}