using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.EasyPost.Domain
{
    /// <summary>
    /// Represents custom exception details
    /// </summary>
    public class EasyPostException
    {
        /// <summary>
        /// Gets or sets the exception
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public CustomHttpException Exception { get; set; }

        /// <summary>
        /// Represents custom HTTP exception details
        /// </summary>
        public class CustomHttpException
        {
            /// <summary>
            /// Gets or sets the code
            /// </summary>
            [JsonProperty(PropertyName = "code")]
            public string Code { get; set; }

            /// <summary>
            /// Gets or sets the message
            /// </summary>
            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            /// <summary>
            /// Gets or sets the errors
            /// </summary>
            [JsonProperty(PropertyName = "errors")]
            public List<CustomError> Errors { get; set; }

            /// <summary>
            /// Represents custom error
            /// </summary>
            public class CustomError
            {
                /// <summary>
                /// Gets or sets the code
                /// </summary>
                [JsonProperty(PropertyName = "code")]
                public string Code { get; set; }

                /// <summary>
                /// Gets or sets the field
                /// </summary>
                [JsonProperty(PropertyName = "field")]
                public string Field { get; set; }

                /// <summary>
                /// Gets or sets the suggestion
                /// </summary>
                [JsonProperty(PropertyName = "suggestion")]
                public string Suggestion { get; set; }

                /// <summary>
                /// Gets or sets the message
                /// </summary>
                [JsonProperty(PropertyName = "message")]
                public List<string> Message { get; set; }
            }
        }
    }
}