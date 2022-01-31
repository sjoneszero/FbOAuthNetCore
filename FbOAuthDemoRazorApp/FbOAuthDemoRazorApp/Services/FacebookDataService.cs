using FbOAuthDemoRazorApp.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Services
{
    /// <summary>
    /// Service that retrieves data from the facebook API once valid access token is present
    /// </summary>
    public class FacebookDataService : IFacebookDataService
    {

        private readonly HttpClient _httpClient;
        private const string _meUrl = "https://graph.facebook.com/me?fields={0}&access_token={1}";

        public FacebookDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get the data for the current user
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<FacebookData> Get(string accessToken)
        {
            var response = await _httpClient.GetAsync(
                string.Format(_meUrl, Helper.JoinPropertiesForDataQuery(typeof(FacebookData)), accessToken)
            );
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<FacebookData>(responseJson);
        }
    }
}
