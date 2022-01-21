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
    public class SessionsController : Controller
    {  
        private readonly IUsersService usersService;
        private readonly ISessionService sessionService;

        /// <summary>
        /// кол-во строк
        /// </summary>
        private int rows = 0;
        /// <summary>
        /// Кол-во записей
        /// </summary>
        private int count = 0;
        /// <summary>
        /// Текст ошибки
        /// </summary>
        private static string error = "";

        Dictionary<int, Breadcrumb> _breadcrumb = new Dictionary<int, Breadcrumb>();

        public SessionsController(IUsersService usersService, ISessionService sessionService)
        {            
            this.usersService = usersService;
            this.sessionService = sessionService;
        }
        [Authorize]
        public IActionResult Index(string userid = "", int page = 1)
        {
            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Сессии пользователей", Value = "Сессии" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;

            var logonUserId = int.Parse(Request.Cookies["id"]);
            var logonUser = usersService.GetById(logonUserId).GetRole();
            ViewBag.UserId = logonUserId;
            ViewBag.Title = "Eips HelpDesk | Список обращений";

            IEnumerable<Sessions> items = null;

            var isUserId = int.TryParse(userid, out int i);

            rows = 25;
            items = sessionService.GetList(page, rows);
            count = sessionService.Count();

            PageViewModel pageViewModel = new PageViewModel(count, page, rows);
            IssueViewModel viewModel = new IssueViewModel
            {
                PageViewModel = pageViewModel,
                Sessions = items,
                User = logonUser
            };

            return View(viewModel);
        }
    }
}
