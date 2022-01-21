using Dapper;
using EipsHelpDesk.Entities;
using EipsHelpDesk.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EipsHelpDesk.Helpers
{
	public static class IssueExtensions
	{
		#region GetCreateds - Кто создал создал
		public static IEnumerable<Issue> GetCreateds(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetCreated());
		}

		public static Issue GetCreated(this Issue issue)
		{
			issue.Created = GetUser(issue.createdbyid);
			return issue;
		}

		#endregion

		#region GetModifieds - Кто изменил
		public static IEnumerable<Issue> GetModifieds(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetModified());
		}

		public static Issue GetModified(this Issue issue)
		{
			issue.Modified = GetUser(issue.modifiedbyid);
			return issue;
		}

		#endregion

		#region GetResponsibles - Ответственные
		/// <summary>
		/// Получить ответственных
		/// </summary>
		/// <param name="issues">Обращения</param>
		/// <returns>ответственные</returns>
		public static IEnumerable<Issue> GetResponsibles(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetResponsible());
		}

		/// <summary>
		/// Получить ответственного
		/// </summary>
		/// <param name="issue">Обращение</param>
		/// <returns>ответственный</returns>
		public static Issue GetResponsible(this Issue issue)
		{
			if (issue.responsibleid != null)
			{
				issue.Responsible = GetUser((int)issue.responsibleid);
				return issue;
			}
			else
			{
				issue.Responsible = null;
				return issue;
			}
		}
		#endregion

		#region GetInitiators - Инициатор
		/// <summary>
		/// Получить инициаторов
		/// </summary>
		/// <param name="issues">Обращения</param>
		/// <returns>иницаторы</returns>
		public static IEnumerable<Issue> GetInitiators(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetInitiator());
		}
		
		/// <summary>
		/// Получить инициатора
		/// </summary>
		/// <param name="issue">Обращение</param>
		/// <returns>иницатор</returns>
		public static Issue GetInitiator(this Issue issue)
		{
			issue.Initiator = GetUser(issue.initiatorid);
			return issue;
		}
		#endregion

		#region GetCategorys - Категории
		/// <summary>
		/// Получить категории
		/// </summary>
		/// <param name="issues">Обращения</param>
		/// <returns>категории</returns>
		public static IEnumerable<Issue> GetCategorys(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetCategory());
		}
		
		/// <summary>
		/// Получить категорию
		/// </summary>
		/// <param name="issue">Обращение</param>
		/// <returns>категория</returns>
		public static Issue GetCategory(this Issue issue)
		{
			issue.Category = Category((int)issue.categoryid);
			return issue;
		}
		#endregion

		#region GetStatuses - Статусы
		/// <summary>
		/// Получить статусы обращений
		/// </summary>
		/// <param name="issues">Обращения</param>
		/// <returns>статусы обращений</returns>
		public static IEnumerable<Issue> GetStatuses(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetStatus());
		}

		/// <summary>
		/// Получить статус обращения
		/// </summary>
		/// <param name="issue">Обращение</param>
		/// <returns>статус обращения</returns>
		public static Issue GetStatus(this Issue issue)
		{
			issue.Status = Status((int)issue.statusid);
			return issue;
		}
		#endregion

		#region Department - Отдел(ы) инициатора(ов)
		public static IEnumerable<Issue> GetDepartments(this IEnumerable<Issue> issues)
		{
			return issues.Select(x => x.GetDepartment());
		}
		public static Issue GetDepartment(this Issue issue)
		{
			issue.Department = Department((int)issue.departmentid);
			return issue;
		}
		#endregion

		#region Private methods

		private static Department Department(int id)
		{
			string query = "Select * from departments Where id = " + id + ";";
			using (NpgsqlConnection db = new NpgsqlConnection(Startup.connection))
			{
				return db.Query<Department>(query).FirstOrDefault();
			}
		}

		private static Status Status(int id)
		{
			string query = "SELECT * FROM status Where id = " + id + ";";
			using (NpgsqlConnection db = new NpgsqlConnection(Startup.connection))
			{
				return db.Query<Status>(query).FirstOrDefault();
			}
		}		

		private static Categories Category(int id)
		{
			string query = "SELECT * FROM categories Where id = " + id + ";";
			using (NpgsqlConnection db = new NpgsqlConnection(Startup.connection))
			{
				return db.Query<Categories>(query).FirstOrDefault();
			}
		}

		

		private static User GetUser(int id)
		{
			string query = "Select * from users Where id = " + id + ";";
			using (NpgsqlConnection db = new NpgsqlConnection(Startup.connection))
			{
				return db.Query<User>(query).FirstOrDefault();
			}
		}

		#endregion
	}
}
