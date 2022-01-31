using FbOAuthDemoRazorApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FbOAuthDemoRazorApp.Controllers.MVC
{

    /// <summary>
    /// Controller that provides the login related actions and views
    /// </summary>
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IFacebookAuthService _facebookAuthService;
        public LoginController(IFacebookAuthService facebookAuthService)
        {
            _facebookAuthService = facebookAuthService;
        }

        /// <summary>
        /// Returns a view with a facebook login link
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["loginUrl"] = _facebookAuthService.LoginUrl(); 
            return View();
        }        
    }
}
