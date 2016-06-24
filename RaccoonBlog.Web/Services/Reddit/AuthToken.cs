using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace RaccoonBlog.Web.Services.Reddit
{
    public class AuthToken
    {
        public AuthToken()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow > CreatedAt.AddSeconds(ExpiresIn);
    }
}