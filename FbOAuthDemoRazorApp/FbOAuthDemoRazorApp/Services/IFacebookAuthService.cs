using FbOAuthDemoRazorApp.Models.Response;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Services
{
    public interface IFacebookAuthService
    {
        Task<bool> DebugToken(string token);
        Task<FacebookAccessTokenResponse> GetAccessToken(string code);
        string LoginUrl(string state = null);
    }
}