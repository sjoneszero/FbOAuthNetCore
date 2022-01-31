namespace FbOAuthDemoRazorApp.Models
{
    public interface IAppSettings
    {
        string AppUrl { get; set; }
        string JwtCookieName { get; set; }
        string JwtSecret { get; set; }
    }
}