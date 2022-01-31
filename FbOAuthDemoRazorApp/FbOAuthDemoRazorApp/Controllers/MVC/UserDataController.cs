using FbOAuthDemoRazorApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FbOAuthDemoRazorApp.Controllers.MVC
{
    /// <summary>
    /// Controller that provides the data related actions and views
    /// </summary>
    [Route("[controller]")]
    public class UserDataController : DataControllerBase
    {

        private readonly IFacebookDataService _facebookDataService;

        public UserDataController(
            IHttpContextAccessor httpContextAccessor, 
            IFacebookDataService facebookDataService) 
            : base(httpContextAccessor)
        {
            _facebookDataService = facebookDataService;
        }
        /// <summary>
        /// Gets user data of logged in user
        /// </summary>
        /// <returns>Facebook data object</returns>
        public async Task<IActionResult> Index()
        {
            var userData = await _facebookDataService.Get(_userContext.Token); 

            return View(userData);
        }
    }
}
