using Newtonsoft.Json;
using Nop.Core.Domain.Messages;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class ContactInfoDto : BaseContactInfoDto
{
    [JsonProperty("identifiers")] public List<Identifier> Identifiers { get; set; }

    #region Nested class

    public class Identifier
    {
        public Identifier(NewsLetterSubscription subscription, string inactiveStatus, bool sendWelcomeMessage = false)
        {
            Id = subscription.Email;
            SendWelcomeMessage = sendWelcomeMessage;
            Channels = new(subscription, inactiveStatus);
        }

        public Identifier()
        {
        }

        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("sendWelcomeMessage")] public bool SendWelcomeMessage { get; set; }
        [JsonProperty("channels")] public EmailChannel Channels { get; }

        #region Nested class

        public class EmailChannel
        {
            public EmailChannel(NewsLetterSubscription subscription, string inactiveStatus)
            {
                Email = new Channel(subscription, inactiveStatus);
            }

            public EmailChannel()
            {
            }

            [JsonProperty("email")] public Channel Email { get; set; }

            #region Nested class

            public class Channel
            {
                public Channel(NewsLetterSubscription subscription, string inactiveStatus)
                {
                    Status = subscription.Active ? "subscribed" : inactiveStatus;
                    StatusDate = subscription.CreatedOnUtc.ToDtoString();
                }

                public Channel()
                {
                }

                [JsonProperty("status")] public string Status { get; set; }
                [JsonProperty("statusDate")] public string StatusDate { get; set; }
            }

            #endregion
        }

        #endregion
    }

    #endregion
}