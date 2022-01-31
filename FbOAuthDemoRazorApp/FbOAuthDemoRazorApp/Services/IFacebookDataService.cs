using FbOAuthDemoRazorApp.Models;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Services
{
    public interface IFacebookDataService
    {
        Task<FacebookData> Get(string accessToken);
    }
}