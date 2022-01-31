namespace FbOAuthDemoRazorApp.Models
{
    /// <summary>
    /// App related settings that get pulled from the appsettings.json
    /// </summary>
    public class AppSettings : IAppSettings
    {
        public string AppUrl { get; set; }

        public string JwtSecret { get; set; }

        public string JwtCookieName { get; set; }
    }
}
