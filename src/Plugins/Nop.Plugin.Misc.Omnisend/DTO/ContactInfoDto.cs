using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class ContactInfoDto : BaseContactInfoDto
{
    [JsonProperty("identifiers")] public List<Identifier> Identifiers { get; set; }

    #region Nested class

    public class Identifier
    {
        public Identifier(string email, bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus, bool sendWelcomeMessage = false)
        {
            Id = email;
            SendWelcomeMessage = sendWelcomeMessage;
            Channels = new(isSubscriptionActive, createdOnUtc, inactiveStatus);
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
            public EmailChannel(bool isSubscriptionActive, DateTime createdOnUtc, string inactiveStatus)
            {
                Email = new Channel(isSubscriptionActive, createdOnUtc, inactiveStatus);
            }

            public EmailChannel()
            {
            }

            [JsonProperty("email")] public Channel Email { get; set; }

            #region Nested class

            public class Channel
            {
                public Channel(bool isSubscriptionActive, DateTime createdOnUtc,  string inactiveStatus)
                {
                    Status = isSubscriptionActive ? "subscribed" : inactiveStatus;
                    StatusDate = createdOnUtc.ToDtoString();
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