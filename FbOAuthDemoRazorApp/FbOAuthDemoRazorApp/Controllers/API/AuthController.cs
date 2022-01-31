using FbOAuthDemoRazorApp.Models;
using FbOAuthDemoRazorApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Controllers.API
{
    /// <summary>
    /// Web API controller to OAuth2 token flow to log the user in
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IAppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(
            IHttpContextAccessor httpContextAccessor,
            IFacebookAuthService facebookAuthService,
            IAppSettings appSettings)
        {
            _facebookAuthService = facebookAuthService;
            _appSettings = appSettings;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Callback method that is called after user is prompted for login credentials
        /// </summary>
        /// <param name="code">The secret code returned by the auth service</param>
        /// <param name="state">The state parameter if previously provided in the login request</param>
        /// <param name="errorReason">Error reason in case the login fails</param>
        /// <param name="error">Error in case the login fails</param>
        /// <param name="errorDescription">Error description in case the login fails</param>
        /// <returns></returns>
        [Route("fblogin")]
        [HttpGet]
        public async Task<IActionResult> LoginCallback(
            [FromQuery] string code = null,
            [FromQuery] string state = null,
            [FromQuery(Name = "error_reason")] string errorReason = null,
            [FromQuery(Name = "error")] string error = null,
            [FromQuery(Name = "error_description")] string errorDescription = null
            )
        {
            if (!string.IsNullOrEmpty(error))
                return Unauthorized(); 

            var accessTokenResponse = await _facebookAuthService.GetAccessToken(code);
            if (await _facebookAuthService.DebugToken(accessTokenResponse.AccessToken))
            {
                // Currently only the token is added to the JWT. Other user data could also be added to the UserContext if required
                var userContext = new UserContext
                {
                    Token = accessTokenResponse.AccessToken
                };
                AttachJwtCookieToReponse(userContext, DateTime.Now.AddSeconds(accessTokenResponse.ExpiresIn));
                return Redirect(_appSettings.AppUrl + "userdata"); 
            }
            else return Unauthorized();
        }

        /// <summary>
        /// Generates a JWT token
        /// </summary>
        /// <param name="userContext">The user context object to include in the JWT</param>
        /// <param name="expires">The expiration of the cookie and JWT</param>
        /// <returns></returns>
        private string GenerateJwtToken(UserContext userContext, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {

                        new Claim("UserContext", JsonConvert.SerializeObject(
                            userContext,
                            new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }
                            )),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iss, _appSettings.AppUrl),
                        new Claim(JwtRegisteredClaimNames.Aud, _appSettings.AppUrl)

                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Attach a new cookie to the response containing the JWT
        /// </summary>
        /// <param name="userContext">The user context object to include in the JWT</param>
        /// <param name="expires">The expiration of the cookie and JWT</param>
        private void AttachJwtCookieToReponse(UserContext userContext, DateTime expires)
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies.Keys.Any(c => c.Equals(_appSettings.JwtCookieName)))
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(_appSettings.JwtCookieName);
            }
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                 _appSettings.JwtCookieName,
                 GenerateJwtToken(userContext, expires),
                 new CookieOptions
                 {
                     HttpOnly = true,
                     //Secure = true,
                     Expires = expires,
                     Domain = new Uri(_appSettings.AppUrl).Host
                 });
        }
    }
}
