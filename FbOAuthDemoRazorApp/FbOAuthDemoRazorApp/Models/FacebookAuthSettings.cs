using System.Collections.Generic;

namespace FbOAuthDemoRazorApp.Models
{
    /// <summary>
    /// Facebook auth related settings that get pulled from the appsettings.json
    /// </summary>
    public class FacebookAuthSettings : IFacebookAuthSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public IEnumerable<string> PermissionsScope { get; set; }
        public string CallbackUrl { get; set; }
    }
}
