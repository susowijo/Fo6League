using League.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Web.Security;
using System.Net;
using System.Data.SqlClient;

namespace League.Controllers
{
   
    public class AccountController : Controller
    {
        // private UserManager<Utilisateur> userManager;
        private LeagueDBEntities db = new LeagueDBEntities();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();

        }

        [AllowAnonymous]
        [HttpPost]

        public async Task<ActionResult> Login(Account login, string ReturnUrl)
        {

            if (string.IsNullOrEmpty(login.Pseudo))
            {
                ViewBag.Error = "Le pseudo est obligatoire !";
            }
            else if (string.IsNullOrEmpty(login.Password))
            {
                ViewBag.Error = "Le mot de passe est obligatoire !";
            }
            else
            {
                var password = db.Sp_Encrypt(login.Password).First();
                Account user = db.Accounts.FirstOrDefault(person => person.Pseudo.Equals(login.Pseudo)
                                                         && person.Password.Equals(password));
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Pseudo, true);
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        return Redirect(ReturnUrl);

                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Compte ou mot de passe incorrect !";
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            //FormsAuthentication.SignOut();
            // await signInManager.SignOutAsync();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<ActionResult> Forgoutpassword()
        {
            return View();
        }

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    //var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie); --> this is where I want to get rid of UserManager
        //    List<Claim> claims = new List<Claim>{
        //    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Name), //user.Name from my database
        //    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id), //user.Id from my database
        //    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "MyApplication"),
        //    new Claim("FirstName", user.FirstName) //user.FirstName from my database
        //};
        //    ClaimsIdentity identity = new System.Security.Claims.ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);

        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}
    }
}