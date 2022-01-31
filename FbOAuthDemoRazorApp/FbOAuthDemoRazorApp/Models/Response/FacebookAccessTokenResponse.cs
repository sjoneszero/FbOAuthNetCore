using Newtonsoft.Json;

namespace FbOAuthDemoRazorApp.Models.Response
{
    /// <summary>
    /// Represents reponse containing Oauth2 token information
    /// </summary>
    public class FacebookAccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

    }
}
