using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AccessControl.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace AccessControl.Controllers
{
    /// <summary>
    /// default controller
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// default welcome page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// privacy
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// error
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns></returns>
        public IActionResult SignOut()
        {

            return new SignOutResult(new[] { "OpenIdConnect", "Cookies" ,"Bearer"});
            //var callbackUrl = Url.Page("/home/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
            //return SignOut(
            //    new AuthenticationProperties { RedirectUri = callbackUrl },
            //    CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme
            //);
        }
        /// <summary>
        /// Signed out page redirect to index
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult SignedOut()
        {
            return Ok();
        }

        


    }
}
