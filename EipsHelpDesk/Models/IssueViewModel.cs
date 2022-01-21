using EipsHelpDesk.Entities;
using EipsHelpDesk.Services;

using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;

namespace EipsHelpDesk.Models
{
	public class IssueViewModel
	{
		/// <summary>
		/// Список пользователей
		/// </summary>
		public IEnumerable<User> Users { get; set; }
		/// <summary>
		/// Пользоваетль
		/// </summary>
		public User User { get; set; }
		/// <summary>
		/// Список ссесий пользователей
		/// </summary>
		public IEnumerable<Sessions> Sessions { get; set; }
		/// <summary>
		/// Список обращений
		/// </summary>
		public IEnumerable<Issue> Issues { get; set; }
		/// <summary>
		/// Обращение
		/// </summary>
		public Issue Issue { get; set; }
		/// <summary>
		/// Информация о пагинации
		/// </summary>
		public PageViewModel PageViewModel { get; internal set; }
		public SelectList SelectListFirstFilters { get; set; }
		public SelectList SelectListSecondFilter { get; set; }
		public SelectList SelectListThirdFilter { get; set; }		
		public int ff { get; set; }
		public int sf { get; set; }
		public int tf { get; set; }
		public IEnumerable<Responsible> Responsibles { get; set; }
		public IEnumerable<Status> Statuses { get; set; }
	}
}
