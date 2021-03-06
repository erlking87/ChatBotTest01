﻿namespace Microsoft.Bot.Connector
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    /// <summary>
    /// Object describing a media thumbnail
    /// </summary>
    public partial class ThumbnailUrl
    {
        /// <summary>
        /// Initializes a new instance of the ThumbnailUrl class.
        /// </summary>
        public ThumbnailUrl() { }

        /// <summary>
        /// Initializes a new instance of the ThumbnailUrl class.
        /// </summary>
        public ThumbnailUrl(string url = default(string), string alt = default(string))
        {
            Url = url;
            Alt = alt;
        }

        /// <summary>
        /// url pointing to an thumbnail to use for media content
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Alt text to display for screen readers on the thumbnail image
        /// </summary>
        [JsonProperty(PropertyName = "alt")]
        public string Alt { get; set; }

    }
}