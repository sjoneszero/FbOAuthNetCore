using FbOAuthDemoRazorApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly IAppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IAppSettings appSettings)
        {
            Next = next;
            _appSettings = appSettings;
        }
        /// <summary>
        /// Method that gets called for each incoming HTTP request and checks for a cookie containing a JWT 
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            if (context.Request.Cookies.TryGetValue(_appSettings.JwtCookieName, out string jwtToken))
            {
                if (jwtToken != null && jwtToken != string.Empty)
                {
                    AttachUserContext(context, jwtToken);
                }
            }
            await Next(context);
        }
        /// <summary>
        /// The token is validated based on specified parameters
        /// If the token is valid, the user context data is extracted and assigned to the context items
        /// </summary>
        private void AttachUserContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _appSettings.AppUrl,
                    ValidateAudience = true,
                    ValidAudience = _appSettings.AppUrl,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var claims = (JwtSecurityToken)validatedToken;
                var userContext = claims.Claims.FirstOrDefault(x => x.Type == "UserContext").Value;
                if (userContext != null)
                {
                    context.Items["UserContext"] = (UserContext)JsonConvert.DeserializeObject(userContext, typeof(UserContext));
                }
            }
            catch
            {
                // Do nothing if validation fails. When property set up, the user won't have access to anything since the user context will not be valid
                // TODO: Need to ensure this is handled if cookie is invalid
            }
        }

    }
}
