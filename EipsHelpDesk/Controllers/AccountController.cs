using Dapper;
using EipsHelpDesk.Entities;
using EipsHelpDesk.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using EipsHelpDesk.Helpers;
using EipsHelpDesk.Services;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace EipsHelpDesk.Controllers
{
	public class AccountController : Controller
	{
        private readonly ILogger<AccountController> logger;
        private readonly IUsersService usersService;
        private readonly ISessionService sessionService;

        public AccountController(ILogger<AccountController> logger, IUsersService usersService, ISessionService sessionService)
        {
            this.logger = logger;   
            this.usersService = usersService;
            this.sessionService = sessionService;
        }

        public ActionResult Login(string returnUrl = null)
		{
			return View(new LoginModel { ReturnUrl = returnUrl });
		}

		[HttpGet]
		public ActionResult AccessDenied()
		{
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Sessions session = new Sessions();

            if (ModelState.IsValid)
            {
                model.Login = model.Login.Replace("@nlmk.com", "");
                User user = usersService.Authentication(model);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                    };
                    //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "EipsHelpDeskCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                    // установка аутентификационных куки
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    Response.Cookies.Append("id", user.Id.ToString());
                    Response.Cookies.Append("login", user.Login);

                    session.createdon = DateTime.Now;
                    session.userid = user.Id;
                    session.login = user.Login;
                    session.action = "login";
                    session.result = true;
                    sessionService.Create(session);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }

                session.createdon = DateTime.Now;
                session.userid = 0;
                session.login = model.Login;
                session.action = "login";
                session.result = false;
                sessionService.Create(session);
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        public async Task<ActionResult> LogoutAsync(int id)
        {
            var user = usersService.GetById(id);

			Sessions session = new Sessions();
			session.createdon = DateTime.Now;
			session.userid = user.Id;
			session.login = user.Login;
			session.action = "logout";
			session.result = true;
			sessionService.Create(session);
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("login", "account");
        }

		private ClaimsIdentity GetIdentity(User user)
		{			
			if (user != null)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                };                
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                // установка аутентификационных куки
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                Response.Cookies.Append("id", user.Id.ToString());
                Response.Cookies.Append("login", user.Login);

                return claimsIdentity;
			}
			return null;
		}

		private async Task SetCooke(User user)
        {    
            // создаем claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                //new Claim(ClaimsIdentity.DefaultNameClaimType, user.Token.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),               
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));            
            Response.Cookies.Append("id", user.Id.ToString());
            Response.Cookies.Append("login", user.Login);
        }
    }
}
