using FbOAuthDemoRazorApp.Models;
using FbOAuthDemoRazorApp.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Services
{
    /// <summary>
    /// Service that provides facebook authentication logic
    /// </summary>
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string _loginUrlBase = "https://www.facebook.com/v12.0/dialog/oauth?client_id={0}&redirect_uri={1}&state={2}&scope={3}";
        private const string _tokenRequestUrlbase = "https://graph.facebook.com/v12.0/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";
        private const string _debugTokenUrlbase = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}";
        private readonly HttpClient _httpClient;
        private readonly IFacebookAuthSettings _facebookAuthSettings;

        public FacebookAuthService(IFacebookAuthSettings facebookAuthSettings, HttpClient httpClient = null)
        {
            _httpClient = httpClient;
            _facebookAuthSettings = facebookAuthSettings;
        }

        /// <summary>
        /// Returns the login url for facebook based on current app settings
        /// </summary>
        /// <param name="state"></param>
        /// <returns>A string representing the login url</returns>
        public string LoginUrl(string state = null)
        {
            return GetLoginUrl(state);
        }
        /// <summary>
        /// Get the access token once the facebook user authentication has been completed
        /// </summary>
        /// <param name="code">The code returned to the callback URL by facebook</param>
        /// <returns>Access token information</returns>
        public async Task<FacebookAccessTokenResponse> GetAccessToken(string code)
        {
            var response = await _httpClient.GetAsync(GetAccessTokenUrl(code));
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<FacebookAccessTokenResponse>(responseJson);
        }

        /// <summary>
        /// Debugs token to ensure it's valid
        /// </summary>
        /// <param name="token">The access token in question</param>
        /// <returns>True if valid, false if not</returns>
        public async Task<bool> DebugToken(string token)
        {
            var debugToken = await _httpClient.GetAsync(GetDebugTokenUrl(token)).ConfigureAwait(false);
            // TODO: Further enhancement could be made to analysis the response data. However, for now, checking the status code will suffice
            return debugToken.IsSuccessStatusCode;
        }


        /// <summary>
        /// Returns the facebook login URL based on current auth settings 
        /// </summary>
        /// <param name="state">State optional</param>
        /// <returns></returns>
        private string GetLoginUrl(string state = null)
        {

            return string.Format(
                _loginUrlBase, _facebookAuthSettings.AppId, _facebookAuthSettings.CallbackUrl, state ?? "0", string.Join(",", _facebookAuthSettings.PermissionsScope));
        }

        /// <summary>
        /// Returns the facebook access token request URL based on current auth settings 
        /// </summary>
        /// <param name="code">The code returned by the facebook authentication process</param>
        /// <returns></returns>
        private string GetAccessTokenUrl(string code)
        {
            return string.Format(
                _tokenRequestUrlbase, _facebookAuthSettings.AppId, _facebookAuthSettings.CallbackUrl, _facebookAuthSettings.AppSecret, code);
        }

        /// <summary>
        /// Returns the debug URL based on the current auth settings
        /// </summary>
        /// <param name="token">The access token</param>
        /// <returns></returns>
        private string GetDebugTokenUrl(string token)
        {
            return string.Format(
            _debugTokenUrlbase, token, $"{_facebookAuthSettings.AppId}|{_facebookAuthSettings.AppSecret}"
            );
        }
    }
}
