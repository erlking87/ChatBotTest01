﻿namespace Microsoft.Bot.Connector
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    /// <summary>
    /// MediaUrl data
    /// </summary>
    public partial class MediaUrl
    {
        /// <summary>
        /// Initializes a new instance of the MediaUrl class.
        /// </summary>
        public MediaUrl() { }

        /// <summary>
        /// Initializes a new instance of the MediaUrl class.
        /// </summary>
        public MediaUrl(string url = default(string), string profile = default(string))
        {
            Url = url;
            Profile = profile;
        }

        /// <summary>
        /// Url for the media
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional profile hint to the client to differentiate multiple
        /// MediaUrl objects from each other
        /// </summary>
        [JsonProperty(PropertyName = "profile")]
        public string Profile { get; set; }

    }
}