using Newtonsoft.Json;

namespace FbOAuthDemoRazorApp.Models
{
    /// <summary>
    /// Represents facebook user data for a specific user
    /// </summary>
    public class FacebookData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("hometown")]
        public Hometown Hometown { get; set; }
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
    }

    public class Hometown
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
