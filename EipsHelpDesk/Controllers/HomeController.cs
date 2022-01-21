using EipsHelpDesk.Helpers;
using EipsHelpDesk.Models;
using EipsHelpDesk.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> logger;
		private readonly IUsersService usersService;	
		private readonly IReportService reportService;
		private readonly IStatusService statusService;

		Dictionary<int, Breadcrumb> _breadcrumb = new Dictionary<int, Breadcrumb>();
		public HomeController(ILogger<HomeController> logger, IUsersService usersService, IReportService reportService, IStatusService statusService)
		{
			this.logger = logger;
			this.usersService = usersService;
			this.reportService = reportService;
			this.statusService = statusService;
		}

		[Authorize]
		public IActionResult Index()
		{
			_breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
			
			ViewBag.Title = "Eips HelpDesk | Главная";
			var userid = int.Parse(Request.Cookies["id"]);
			var user = usersService.GetById(userid).GetRole();
			ViewBag.UserId = user.Id;
			var report1 = reportService.GetResponsibles();
			var _statuses = statusService.GetList();

			IssueViewModel viewModel = new IssueViewModel
			{
				User = user,
				Responsibles = report1,
				Statuses = _statuses,
			};

			return View(viewModel);
		}

		public IActionResult Privacy()
		{
			var userid = int.Parse(Request.Cookies["id"]);
			var user = usersService.GetById(userid).GetRole();
			ViewBag.UserId = user.Id;

			IssueViewModel viewModel = new IssueViewModel
			{
				User = user
			};

			return View(viewModel);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			ViewBag.UserId = Request.Cookies["id"];
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
