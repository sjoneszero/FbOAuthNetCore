using System.Collections.Generic;

namespace FbOAuthDemoRazorApp.Models
{
    public interface IFacebookAuthSettings
    {
        string AppId { get; set; }
        string AppSecret { get; set; }
        string CallbackUrl { get; set; }
        IEnumerable<string> PermissionsScope { get; set; }
    }
}