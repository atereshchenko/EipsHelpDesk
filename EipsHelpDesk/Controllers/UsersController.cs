using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EipsHelpDesk.Entities;
using EipsHelpDesk.Helpers;
using EipsHelpDesk.Models;
using EipsHelpDesk.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EipsHelpDesk.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IIssueService issueService;

        /// <summary>
        /// кол-во строк
        /// </summary>
        int rows = 0;
        /// <summary>
        /// Кол-во записей
        /// </summary>
        int count = 0;
        Dictionary<int, Breadcrumb> _breadcrumb = new Dictionary<int, Breadcrumb>();
        public UsersController(IUsersService usersService, IIssueService issueService)
        {            
            this.usersService = usersService;
            this.issueService = issueService;
        }

        [Authorize(Roles = "Administrators")]
        public IActionResult Index(string userid = "", int page = 1)
        {
            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Список пользователей", Value = "Пользователи" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;

            var logonUserId = int.Parse(Request.Cookies["id"]);
            var logonUser = usersService.GetById(logonUserId).GetRole();
            ViewBag.UserId = logonUserId;
            ViewBag.Title = "Eips HelpDesk | Список пользователей";

            IEnumerable<User> items = null;
            
            items = usersService.GetList();
            
            IssueViewModel viewModel = new IssueViewModel
            {                
                Users = items,
                User = logonUser
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Administrators, Moderators")]
        public IActionResult View(int id)
        {
            //http://lg-d-54a5053332:81/pages/examples/contacts.html
            //http://lg-d-54a5053332:81/pages/examples/profile.html
            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Профиль", Value = "Профиль" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;
            ViewBag.UserId = Request.Cookies["id"];
            ViewBag.Title = "Eips HelpDesk | Просмотр профиля";
            User user = new User();
            if (id == 0)
            {
                user = usersService.GetById(int.Parse(ViewBag.UserId));
            }
            else
            {
                user = usersService.GetById(id).GetRole();
            }

            var tmp1 = issueService.GetByInitiator(user.Id);
            var tmp2 = issueService.GetByResponsible(user.Id);

            IssueViewModel viewModel = new IssueViewModel
            {                          
                User = user,                
                ff = tmp1.Count(),
                sf= tmp2.Count()
            };

            return View(viewModel);
        }        
    }
}
