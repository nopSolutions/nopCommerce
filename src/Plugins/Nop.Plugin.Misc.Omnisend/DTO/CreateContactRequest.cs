using Newtonsoft.Json;
using Nop.Core.Domain.Messages;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class CreateContactRequest : ContactInfoDto, IBatchSupport
{
    public CreateContactRequest(NewsLetterSubscription subscription, string inactiveStatus,
        bool sendWelcomeMessage)
    {
        Identifiers = new List<Identifier> { new(subscription, inactiveStatus, sendWelcomeMessage) { Type = "email" } };
        SendWelcomeEmail = sendWelcomeMessage;
    }

    [JsonProperty("sendWelcomeEmail")] public bool SendWelcomeEmail { get; }
}