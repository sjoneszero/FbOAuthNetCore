using FbOAuthDemoRazorApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FbOAuthDemoRazorApp.Controllers.MVC
{
    /// <summary>
    /// Abstract class that all should be inherited by controllers that interact with the Facebook API
    /// </summary>
    public abstract class DataControllerBase : Controller
    {
        /// <summary>
        /// The user data of the logged in user
        /// </summary>
        protected UserContext _userContext;

        protected DataControllerBase(IHttpContextAccessor httpContextAccessor)
        {
            _userContext = (UserContext)httpContextAccessor.HttpContext.Items["UserContext"];
        }
    }
}
