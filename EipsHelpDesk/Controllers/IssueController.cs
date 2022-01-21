using EipsHelpDesk.Entities;
using EipsHelpDesk.Helpers;
using EipsHelpDesk.Models;
using EipsHelpDesk.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EipsHelpDesk.Controllers
{
    public class IssueController : Controller
    {
        private readonly IIssueService issueService;
        private readonly IStatusService statusService;
        private readonly ICategoryService categoryService;        
        private readonly IUsersService usersService;
        private readonly ILifecycleService lifecycleService;
        private readonly IUserssettingsService userssettingsService;
        private readonly ILogger<IssueController> logger;

        /// <summary>
        /// кол-во строк
        /// </summary>
        int rows = 0;
        /// <summary>
        /// Кол-во записей
        /// </summary>
        int count = 0;
        /// <summary>
        /// Текст ошибки
        /// </summary>
        public static string error = "";

        public static string filter = " ";       

        Dictionary<int, Breadcrumb> _breadcrumb = new Dictionary<int, Breadcrumb>();

        public IssueController(ILogger<IssueController> logger, IIssueService issueService, IStatusService statusService, ICategoryService categoryService, IUsersService usersService, ILifecycleService lifecycleService, IUserssettingsService userssettingsService)
        {
            this.logger = logger;
            this.issueService = issueService;
            this.statusService = statusService;
            this.categoryService = categoryService;            
            this.usersService = usersService;
            this.lifecycleService = lifecycleService;
            this.userssettingsService = userssettingsService;
        }

        [Authorize]
        public IActionResult Index(IssueViewModel model, int page = 1 )
        {
            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Обращения", Value = "Обращения" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;

            var userid = int.Parse(Request.Cookies["id"]);
            var user = usersService.GetById(userid).GetRole();

            ViewBag.UserId = userid;
            ViewBag.Title = "Eips HelpDesk | Список обращений";

            //Первая фильтрация списка
            FirstFilter firstFilter = new FirstFilter();
            var _ff = firstFilter.GetList().OrderBy(x => x.id).ToList();
            ViewBag.FirstFilter = _ff.Select(x => new DropDownList { Value = x.id, Text = x.name });

            SecondFilter secondFilter = new SecondFilter();
            var _sf = secondFilter.GetList().OrderBy(x => x.id).ToList();

            ThirdFilter thirdFilter = new ThirdFilter();
            var _tf = thirdFilter.GetList().OrderBy(x => x.id).ToList();

            IEnumerable<Issue> items = null;

            filter = "";
            rows = 25;
			switch (model.ff)
			{
				case 0:
					break;
				case 1:
					filter += " where initiatorid = " + userid.ToString() + " ";
					break;
				case 2:
					filter += " where responsibleid = " + userid.ToString() + " ";
					break;
			}

            switch (model.sf)
            {
                case 0:
                    break;
                case 1:
                    filter += (filter == "") ? " where statusid = 1 " : " and statusid = 1 ";
                    break;
                case 2:
                    filter += (filter == "") ? " where statusid = 2 " : " and statusid = 2 ";
                    break;
                case 3:
                    filter += (filter == "") ? " where statusid = 3 " : " and statusid = 3 ";
                    break;
                case 4:
                    filter += (filter == "") ? " where statusid = 4 " : " and statusid = 4 ";
                    break;
                case 5:
                    filter += (filter == "") ? " where statusid = 5 " : " and statusid = 5 ";
                    break;
                case 6:
                    filter += (filter == "") ? " where statusid <> 3 and planneddate < (now()::timestamp(0) + interval '1 day') " : " and statusid <> 3 and planneddate < (now()::timestamp(0) + interval '1 day') ";
                    break;
            }

            switch (model.tf)
            {
                case 0:
                    filter += " order by id desc ";
                    break;
                case 1:
                    filter += " order by id asc ";
                    break;                
            }

            items = issueService.ToList(page, rows, filter).GetInitiators().GetCategorys().GetStatuses().GetDepartments().GetResponsibles();
            count = items.Count();

            PageViewModel pageViewModel = new PageViewModel(count, page, rows);
            IssueViewModel viewModel = new IssueViewModel
            {
                PageViewModel = pageViewModel,                
                Issues = items,
                User = user,
                SelectListFirstFilters = new SelectList(_ff, "id", "name"),
                SelectListSecondFilter = new SelectList(_sf, "id", "name"),
                SelectListThirdFilter = new SelectList(_tf, "id", "name"),
            };

            return View(viewModel);
        }             

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Обращения", Value = "Обращения", Controller = "issue", Action = "index" });
            _breadcrumb.Add(3, new Breadcrumb() { Id = 3, Name = "Создание", Value = "Создание", Action = "issue", Controller = "сreate" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;
            ViewBag.Title = "Eips HelpDesk | Создание обращения";
            
            var userid = int.Parse(Request.Cookies["id"]);
            var user = usersService.GetById(userid).GetRole();
            ViewBag.UserId = userid;
            
            ViewBag.Category = categoryService.GetList().OrderBy(x => x.Name).Select(x => new DropDownList { Value = x.id, Text = x.Name });
            ViewBag.Initiator = usersService.GetListIsActual(true).OrderBy(x => x.Name).Select(x => new DropDownList { Value = x.Id, Text = x.Name });

            ViewBag.Error = error;
            error = "";

            //var carentcategory = categoryService.GetById(5);

            var _issue = new Issue();
            _issue.initiatorid = userid;
            //_issue.categoryid = carentcategory.id;
            IssueViewModel viewModel = new IssueViewModel
			{
				Issue = _issue,
                User = user
			};
			return View(viewModel);
		}

        [Authorize]
        [HttpPost]
        public IActionResult Create(IssueViewModel model)
        {
            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Обращения", Value = "Обращения", Controller = "issue", Action = "index" });
            _breadcrumb.Add(3, new Breadcrumb() { Id = 3, Name = "Создание", Value = "Создание", Action = "issue", Controller = "сreate" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;

            var userid = int.Parse(Request.Cookies["id"]);
            var user = usersService.GetById(userid).GetRole();
            ViewBag.UserId = userid;
            ViewBag.Title = "Eips HelpDesk | Создание обращения";

            if (model.Issue.categoryid == 0)
            {
                error = "Необходимо заполнить поле \"Категория\"";
                return RedirectToAction("create", "issue");
            }

            if (model.Issue.subject == null)
            {
                error = "Необходимо заполнить поле \"Тема обращения\"";
                return RedirectToAction("create", "issue");
            }

            if (model.Issue.text == null)
            {
                error = "Необходимо заполнить поле \"Текст обращения\"";
                return RedirectToAction("create", "issue");
            }

            if (model.Issue.initiatorid == 0)
            {
                error = "Необходимо заполнить поле \"Инициатор\"";
                return RedirectToAction("create", "issue");
            }

            var _status = statusService.GetById(1);
            var initiator = usersService.GetById(model.Issue.initiatorid);

            model.Issue.createdon = System.DateTime.Now;
            model.Issue.createdbyid = user.Id;
            model.Issue.modifiedon = System.DateTime.Now;
            model.Issue.modifiedbyid = user.Id;
            model.Issue.statusid = _status.id;            

            model.Issue.departmentid = initiator.Departmentid;
            
            var category = categoryService.GetById(model.Issue.categoryid).GetOwner();
            model.Issue.responsibleid = category.Owner.Id;

            var _id = issueService.Create(model.Issue);
            var issue = issueService.GetById(_id).GetStatus();

            //Создаем запись жизненого цикла обращения
            Lifecycle lifecycle = new Lifecycle();
            lifecycle.createdbyid = user.Id;
            lifecycle.modifiedbyid = user.Id;
            lifecycle.issueid = issue.id;
            lifecycle.statusid = issue.Status.id;
            lifecycle.name = issue.Status.name;
            lifecycle.userid = category.Owner.Id;
            var _lifecycle = lifecycleService.Create(lifecycle);

            //Обновляем идентификтаор жизненого цикла обращения
            issue.lifecycleid = _lifecycle;
            var insertIssueLifecycle = issueService.Update(issue);

            //return RedirectToAction("edit", "issue", new { id = issue.id });
            return RedirectToAction("index", "issue");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var _issue = issueService.GetById((int)id).GetInitiator().GetStatus().GetCategory().GetResponsible();

            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Обращения", Value = "Обращения", Controller = "issue", Action = "index" });
            _breadcrumb.Add(3, new Breadcrumb() { Id = 3, Name = "Обращение №" + id.ToString(), Value = id.ToString(), Action = "issue", Controller = "edit" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;
            
            var userid = int.Parse(Request.Cookies["id"]);
            var user = usersService.GetById(userid).GetRole();
            ViewBag.UserId = userid;
            ViewBag.ResponsibleId = _issue.responsibleid;

            ViewBag.PlannedDate = _issue.planneddate?.ToString("yyyy-MM-dd HH:mm");           

            ViewBag.FactDate = _issue.factdate?.ToString("yyyy-MM-dd HH:mm");
            ViewBag.Title = "Eips HelpDesk | Просмотр обращения";

            ViewBag.Category = categoryService.GetList().OrderBy(x => x.Name).Select(x => new DropDownList { Value = x.id, Text = x.Name });
            ViewBag.Status = statusService.GetList().Select(x => new DropDownList { Value = x.id, Text = x.name });

            var tmp = _issue.responsibleid;
            ViewBag.Owner = _issue.Responsible.Name;
            
            ViewBag.Error = error;
            error = "";
            _issue.isaccept = true;

            IssueViewModel viewModel = new IssueViewModel
            {                
                Issue = _issue,
                User = user
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(IssueViewModel viewModel)
        {
            var issue = issueService.GetById(viewModel.Issue.id);
            var modelcategory = categoryService.GetById(viewModel.Issue.categoryid);
            //var newstatus = statusService.GetById(viewModel.Issue.statusid);

            _breadcrumb.Add(1, new Breadcrumb() { Id = 1, Name = "Главная", Value = "Главная", Controller = "home", Action = "index" });
            _breadcrumb.Add(2, new Breadcrumb() { Id = 2, Name = "Обращения", Value = "Обращения", Controller = "issue", Action = "index" });
            _breadcrumb.Add(3, new Breadcrumb() { Id = 3, Name = "Обращение №" + issue.id.ToString(), Value = issue.id.ToString(), Action = "issue", Controller = "edit" });
            List<Breadcrumb> Breadcrumbs = _breadcrumb.Select(kvp => kvp.Value).ToList();
            ViewBag.Breadcrumbs = Breadcrumbs;
            var userid = int.Parse(Request.Cookies["id"]);
            var user = usersService.GetById(userid).GetRole();
            ViewBag.UserId = userid;
            ViewBag.Title = "Eips HelpDesk | Просмотр обращения";

            switch (issue.statusid)            
            {
                #region case 1 New
                case 1:
                    if (user.Role.Id == 1 || user.Role.Id == 2)
                    {
                        //Закрываем текущий lifecycle и создаем новый
                        var newowner = usersService.GetById(modelcategory.ownerid);
                        var newstatus = statusService.GetById(viewModel.Issue.statusid);
                        var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                        //Обновляем обращение
                        issue = viewModel.Issue;
                        issue.modifiedbyid = user.Id;
                        issue.modifiedon = System.DateTime.Now;
                        issue.responsibleid = newowner.Id;                        
                        issue.lifecycleid = newlifecycleId;
                        issue.factdate = null;
                        if (viewModel.Issue.statusid == 5)
                        {
                            issue.factdateclosed = DateTime.Now;
                        }
                        else
                        {
                            issue.factdateclosed = null;
                        }

                        issueService.Update(issue);

                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else if (issue.responsibleid == user.Id)
                    {
                        if (issue.categoryid != modelcategory.id)
                        {
                            issue = viewModel.Issue;
                            var newowner = usersService.GetById(modelcategory.ownerid);
                            var newstatus = statusService.GetById(viewModel.Issue.statusid);
                            var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                            issue.modifiedbyid = user.Id;
                            issue.modifiedon = System.DateTime.Now;
                            issue.lifecycleid = newlifecycleId;
                            issue.responsibleid = modelcategory.ownerid;                            
                            issue.planneddate = null;
                            issueService.Update(issue);
                        }
                        else if (viewModel.Issue.planneddate == null)
                        {
                            error = "Не заполнено поле \"Плановая дата\"";
                        }
                        else
                        {
                            var newowner = usersService.GetById(modelcategory.ownerid);
                            var newstatus = statusService.GetById(2);
                            var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                            issue = viewModel.Issue;
                            issue.modifiedbyid = user.Id;
                            issue.modifiedon = System.DateTime.Now;
                            issue.lifecycleid = newlifecycleId;
                            issue.statusid = newstatus.id;
                            issueService.Update(issue);
                        }
                        return RedirectToAction("index", "issue");
                    }
                    else
                    {
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    
                #endregion
                
                #region case 2 In Prpogress
                case 2:
                    if (user.Role.Id == 1 || user.Role.Id == 2)
                    {
                        //Закрываем текущий lifecycle и создаем новый
                        var newowner = usersService.GetById(modelcategory.ownerid);
                        var newstatus = statusService.GetById(viewModel.Issue.statusid);
                        var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                        //Обновляем обращение
                        issue = viewModel.Issue;
                        issue.modifiedbyid = user.Id;
                        issue.modifiedon = System.DateTime.Now;
                        issue.responsibleid = newowner.Id;                        
                        issue.lifecycleid = newlifecycleId;
                        if (viewModel.Issue.statusid == 5)
                        {
                            issue.factdateclosed = DateTime.Now;
                        }
                        else
                        {
                            issue.factdateclosed = null;
                        }
                        issueService.Update(issue);
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else if (issue.responsibleid == user.Id)
                    {
                        //1. Проверить заполнено ли поле решение
                        if (viewModel.Issue.solution == "" || viewModel.Issue.solution == null)
                        {
                            error = "Не заполнено поле \"Решение\"";
                        }
                        else
                        {
                            issue = viewModel.Issue;
                            issue.modifiedbyid = user.Id;
                            issue.modifiedon = DateTime.Now;
                            issue.factdate = DateTime.Now;

                            //2. Проверить: Принято/Отклонено: cохранить в соответствующую ветку
                            Status status = new Status();
                            if (viewModel.Issue.isaccept)
                            {
                                status = statusService.GetById(3);
                            }
                            else
                            {
                                status = statusService.GetById(4);
                            }
                            issue.statusid = status.id;
                            issue.lifecycleid = CloseOldOpenNew(issue, user, status, user.Id);
                            issueService.Update(issue);
                        }                       
                        
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else
                    {
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }                    
				#endregion

				#region case 3 Fixed
				case 3:
                    if (user.Role.Id == 1 || user.Role.Id == 2)
                    {
                        //Закрываем текущий lifecycle и создаем новый
                        var newowner = usersService.GetById(modelcategory.ownerid);
                        var newstatus = statusService.GetById(viewModel.Issue.statusid);
                        var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                        //Обновляем обращение
                        issue = viewModel.Issue;
                        issue.modifiedbyid = user.Id;
                        issue.modifiedon = System.DateTime.Now;
                        issue.responsibleid = newowner.Id;                        
                        issue.lifecycleid = newlifecycleId;
                        if (viewModel.Issue.statusid == 5)
                        {
                            issue.factdateclosed = DateTime.Now;
                        }
                        else
                        {
                            issue.factdateclosed = null;
                        }
                        issueService.Update(issue);
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else if (issue.responsibleid == user.Id)
                    {                        
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else
                    {
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
				#endregion

				#region case 4 Canceled
				case 4:
                    if (user.Role.Id == 1 || user.Role.Id == 2)
                    {
                        //Закрываем текущий lifecycle и создаем новый
                        var newowner = usersService.GetById(modelcategory.ownerid);
                        var newstatus = statusService.GetById(viewModel.Issue.statusid);
                        var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                        //Обновляем обращение
                        issue = viewModel.Issue;
                        issue.modifiedbyid = user.Id;
                        issue.modifiedon = System.DateTime.Now;
                        issue.responsibleid = newowner.Id;                        
                        issue.lifecycleid = newlifecycleId;
                        if (viewModel.Issue.statusid == 5)
                        {
                            issue.factdateclosed = DateTime.Now;
                        }
                        else
                        {
                            issue.factdateclosed = null;
                        }
                        issueService.Update(issue);
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else if (issue.responsibleid == user.Id)
                    {                       
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else
                    {
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
				#endregion

				#region case 5 Closed
				case 5:
                    if (user.Role.Id == 1 || user.Role.Id == 2)
                    {
                        //Закрываем текущий lifecycle и создаем новый
                        var newowner = usersService.GetById(modelcategory.ownerid);
                        var newstatus = statusService.GetById(viewModel.Issue.statusid);
                        var newlifecycleId = CloseOldOpenNew(issue, user, newstatus, newowner.Id);

                        //Обновляем обращение
                        issue = viewModel.Issue;
                        issue.modifiedbyid = user.Id;
                        issue.modifiedon = System.DateTime.Now;
                        issue.responsibleid = newowner.Id;                        
                        issue.lifecycleid = newlifecycleId;
                        issueService.Update(issue);
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else if (issue.responsibleid == user.Id)
                    {                        
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
                    else
                    {
                        return RedirectToAction("edit", "issue", new { issue.id });
                    }
				#endregion

				default:
                    return RedirectToAction("edit", "issue", new { issue.id });                    
            }            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Закрытие текущего Lifecycle и создание нового
        /// </summary>
        /// <param name="issue">Обращение</param>
        /// <param name="user">кто вносит изменения</param>
        /// <param name="newstatus">новый статус обращения</param>
        /// <param name="ownerid">ответственный</param>
        /// <returns>идентификатор вновь созданного Lifecycle</returns>
        private int CloseOldOpenNew(Issue issue, User user, Status newstatus, int ownerid)
        {
            //Закрываем последний Lifecycle
            var currentlifecycle = lifecycleService.GetByIssueId(issue.id).LastOrDefault();
            currentlifecycle.datefact = System.DateTime.Now;
            lifecycleService.Update(currentlifecycle);

            //Создаем новый Lifecycle
            Lifecycle newlifecycle = new Lifecycle();
            newlifecycle.createdbyid = user.Id;
            newlifecycle.createdon = System.DateTime.Now;
            newlifecycle.modifiedbyid = user.Id;
            newlifecycle.modifiedon = System.DateTime.Now;
            newlifecycle.issueid = issue.id;
            newlifecycle.statusid = newstatus.id;
            newlifecycle.name = newstatus.name;
            newlifecycle.userid = ownerid;
            newlifecycle.id = lifecycleService.Create(newlifecycle);
            return newlifecycle.id;
        }               
    }
}
