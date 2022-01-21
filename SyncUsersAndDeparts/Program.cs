using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Npgsql;
using SqlToHelpDescSync.Postgres;
using SqlToHelpDescSync.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SyncUsersAndDeparts
{
	internal class Program
	{
		/// <summary>
		/// Строка подключения к БД PostgreSQL
		/// </summary>
		public static string connection = "Host=localhost;Port=5432;Database=eipshelpdesk;Username=postgres;Password=A1aaaaaa";
		/// <summary>
		/// URL web-сервиса сотрудников
		/// </summary>
		public static string urlWorkerService = "";
		public static string Start = DateTime.Now.ToString();
		/// <summary>
		/// Токен аутентификации
		/// </summary>
		public static string token = "1c35f65b-6b10-4893-bc63-3a810d11fd5a";
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Синхронизируем пользователей и их отделы...");
			Console.ResetColor();
			SyncUsers();

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Проверяем руководителей отделов...");
			Console.ResetColor();
			ChekDepartments();

			Console.WriteLine("\nStart: " + Start + "; Finish: " + DateTime.Now.ToString());
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
		/// <summary>
		/// Определение руководителей отделов.
		/// </summary>
		/// <exception cref="Exception">ошибка</exception>
		public static void ChekDepartments()
		{
			//1. Получить все отделы в Postgres
			var allDepartments = GetDepartments();
			var allUsers = GetUsers();
			//2. Определить начальника в SQL каждого отдела
			foreach (var department in allDepartments)
			{
				if (department.sqlbossid > 0)
				{
					//ищем пользователя					
					var boss = allUsers.Where(x => x.sqlid == department.sqlbossid).FirstOrDefault();
					if (boss != null)
					{
						Console.Write($"Руководитель отдела \"" + department.name + "\": ");
						//Нужно проверить равенство значений
						if (department.userbossid != boss.id)
						{
							department.userbossid = boss.id;
							//3. Обновить начальника в отделах в Postgres.
							var resultUpdateDepartment = UpdateDepartment(department);
							Console.WriteLine($"данные обновлены: {boss.name}");
						}
						else
						{
							Console.WriteLine($"данные актуальны.");
						}
					}
					else
					{
						//4. Если в Postgres сотрудник отсутствуент - создаем
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine($"Отсутствует руководитель {department.sqlbossid} у отдела {department.id} {department.name}. Добавляем...");
						Console.ResetColor();
						var worker = GetWorker(department.sqlbossid);
						if (worker != null)
						{
							var dateAndTime = new DateTime(0001, 01, 01);
							User newBoss = new User();

							newBoss.sqlid = worker.id;
							newBoss.roleid = 3;
							newBoss.name = worker.name;
							newBoss.personnelnumber = worker.clock_number;
							newBoss.login = worker.login;
							newBoss.positions = worker.job;
							newBoss.departmentid = department.id;
							newBoss.phone = worker.WorkPhone;
							if (worker.sacking_date == dateAndTime)
							{
								newBoss.isactual = true;
							}
							else
							{
								newBoss.isactual = false;
							}
							var resultUpdate = CreateUser(newBoss);
							Console.ForegroundColor = ConsoleColor.Green;
							Console.WriteLine($"Пользовател успешно добавлен с id {resultUpdate}");
							Console.ResetColor();
						}
						else
						{
							throw new Exception($"Не удалось определить руководителя с идентификаторм {department.sqlbossid}");
						}
					}
				}
			}
		}

		/// <summary>
		/// Синхронизация пользователей (добавление, деактивация, обновление) и их отделов.
		/// </summary>
		public static void SyncUsers()
		{
			//Список SQL-пользователей
			var sqlworkers = GetWorkers();
			int[] workerSqlMass = sqlworkers.Select(x => x.id).ToArray();
			Console.WriteLine("Кол-во Sql-пользователей: " + workerSqlMass.Count());

			//Список Postgres-пользователй
			var postgresUsers = GetUsers();
			int[] usersPostgressMass = postgresUsers.Select(x => x.sqlid).ToArray();
			Console.WriteLine("Кол-во Postgres-пользователей: " + usersPostgressMass.Count());

			#region Пользователи для добавления
			var idUsersForAdd = workerSqlMass.Except(usersPostgressMass);
			Console.WriteLine("Необходимо добавить: " + idUsersForAdd.Count() + " новых пользователей");

			if (idUsersForAdd.Count() > 0)
			{
				User user = new User();
				foreach (var idUser in idUsersForAdd)
				{
					var userFromSql = GetWorker(idUser);
					user.sqlid = userFromSql.id;
					user.roleid = ChekUserRole(userFromSql.id);
					user.name = userFromSql.name;
					user.personnelnumber = userFromSql.clock_number;
					user.login = userFromSql.login;
					user.positions = userFromSql.job;
					user.isactual = true;
					user.phone = userFromSql.WorkPhone;

					//Нужно проверить Отдел пользователя у нас в БД
					var userDep = GetDepartment(userFromSql.id_department);
					if (userDep != null)
					{
						user.departmentid = userDep.id;
					}
					else
					{
						var department = GetSubdivision(userFromSql.id_department);

						Department forAdd = new Department();
						forAdd.sqlid = department.Id;
						forAdd.userbossid = 0;
						forAdd.sqlbossid = department.Id_boss;
						forAdd.name = department.Name;
						forAdd.shortname = department.Short_name;
						forAdd.isactual = true;
						var idDepart = CreateDepartment(forAdd);
						Console.WriteLine("Добавлен отдел: " + department.Name);
						user.departmentid = idDepart;
					}
					var userId = CreateUser(user);
					Console.WriteLine("Добавлен пользователь: " + user.name);
				}
			}

			#endregion

			#region Пользователи для деактивации
			var usersForDeactivate = usersPostgressMass.Except(workerSqlMass);
			Console.WriteLine("Необходимо выполнить деактивацию: " + usersForDeactivate.Count() + " пользователей");

			if (usersForDeactivate.Count() > 0)
			{
				foreach (var item in usersForDeactivate)
				{
					User user = new User();
					user = GetUser(item);
					user.isactual = false;
					user.roleid = 3;
					var updateuser = UpdateUser(user);
					Console.WriteLine("Деактивирован: " + user.name);
				}
			}

			#endregion

			#region Пользователи для обновления
			int countUpdate = 0;
			var usersForUpdate = workerSqlMass.Intersect(usersPostgressMass);

			if (usersForUpdate.Count() > 0)
			{
				Console.WriteLine("Необходимо проверить данный пользователей: " + usersForUpdate.Count());
				foreach (var item in usersForUpdate)
				{
					var postgresUser = postgresUsers.Where(x => x.sqlid == item).FirstOrDefault();
					var sqlworker = sqlworkers.Where(x => x.id == item).FirstOrDefault();

					if (postgresUser.name != sqlworker.name || postgresUser.personnelnumber != sqlworker.clock_number || postgresUser.login != sqlworker.login || postgresUser.positions != sqlworker.job || postgresUser.phone != sqlworker.WorkPhone)
					{
						postgresUser.name = sqlworker.name;
						postgresUser.personnelnumber = sqlworker.clock_number;
						postgresUser.login = sqlworker.login;
						postgresUser.positions = sqlworker.job;
						postgresUser.phone = sqlworker.WorkPhone;
						postgresUser.roleid = ChekUserRole(sqlworker.id);
						var resultUpdate = UpdateUser(postgresUser);
						countUpdate++;
						Console.WriteLine("Обновлен пользователь: " + postgresUser.id + " " + postgresUser.name);
					}
				}
				Console.WriteLine("Обновлено данных пользователей: " + countUpdate);
			}
			else
			{
				Console.WriteLine("Пользователей для обновления: " + usersForUpdate.Count());
			}


			#endregion
		}
		/// <summary>
		/// Метод определения роли для сотрудника
		/// </summary>
		/// <param name="id">SQL идентификатор пользователя</param>
		/// <returns></returns>
		public static int ChekUserRole(int id)
		{
			int roleId = 3;
			switch (id)
			{
				case 4744:
					roleId = 1;
					break;
				default:
					roleId = 3;
					break;
			}
			return roleId;
		}

		#region Postgres data
		public static User GetUser(int sqlid)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM public.users Where sqlid = " + sqlid + ";";
				return db.Query<User>(query).FirstOrDefault();
			}
		}
		/// <summary>
		/// Список всех пользователей в Postgres
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<User> GetUsers()
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM public.users;";
				return db.Query<User>(query);
			}
		}
		/// <summary>
		/// Создание пользователя в Postgres
		/// </summary>
		/// <param name="users">объект Пользователь</param>
		/// <returns>идентификатор созданого пользователя</returns>
		public static int CreateUser(User users)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				users.createdon = DateTime.Now;
				users.modifiedon = DateTime.Now;
				var result = (int)db.Insert(users);
				return result;
			}
		}
		/// <summary>
		/// Обновление пользователя в Postgres
		/// </summary>
		/// <param name="users">объект Пользователь</param>
		/// <returns>true/false</returns>
		public static bool UpdateUser(User users)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				users.modifiedon = DateTime.Now;
				var result = db.Update(users);
				return result;
			}
		}
		/// <summary>
		/// Получить список отделов в Postgres
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Department> GetDepartments()
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM public.departments;";
				return db.Query<Department>(query);
			}
		}
		/// <summary>
		/// Отдел в postgres
		/// </summary>
		/// <param name="sqlid">sql идентификатор</param>
		/// <returns></returns>
		public static Department GetDepartment(int sqlid)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				string query = "SELECT * FROM public.departments Where sqlid = " + sqlid + ";";
				return db.Query<Department>(query).FirstOrDefault();
			}
		}

		public static int CreateDepartment(Department department)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				department.createdon = DateTime.Now;
				department.modifiedon = DateTime.Now;
				var result = (int)db.Insert(department);
				return result;
			}
		}

		public static bool UpdateDepartment(Department department)
		{
			using (var db = new NpgsqlConnection(connection))
			{
				department.modifiedon = DateTime.Now;
				var result = db.Update(department);
				return result;
			}
		}
		#endregion

		#region Sql Data
		/// <summary>
		/// Получить список пользователей SQL
		/// </summary>
		/// <returns>Список SQL-пользователей</returns>
		/// <exception cref="Exception">ошибка</exception>
		public static IEnumerable<Workers> GetWorkers()
		{
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Add("X-EIPS-AuthToken", token);
			HttpResponseMessage mes = client.GetAsync(urlWorkerService + "worker?filter=Sacking_date is null and Combine_job_type is NULL and Clock_number > 1").Result;
			if (mes.StatusCode == HttpStatusCode.OK)
			{
				string res = mes.Content.ReadAsStringAsync().Result;
				var json = JsonConvert.DeserializeObject<Response>(res);
				return json.data.workers;
			}
			else
			{
				throw new Exception("Ошибка при работе с модулем WorkerService: не удалось выполнить запрос при попытке получить список Worker-ов");
			}
		}

		private static Workers GetWorker(int id)
		{
			Workers worker = new Workers();

			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Add("X-EIPS-AuthToken", token);
			HttpResponseMessage mes = client.GetAsync(urlWorkerService + "worker/" + id.ToString()).Result;
			if (mes.StatusCode == HttpStatusCode.OK)
			{
				string result = mes.Content.ReadAsStringAsync().Result;
				worker = JsonConvert.DeserializeObject<Workers>(result);
			}
			return worker;
		}

		private static List<Subdivision> GetSubdivisions()
		{
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Add("X-EIPS-AuthToken", token);
			HttpResponseMessage mes = client.GetAsync(urlWorkerService + "Subdivision?filter=IsDepartment = 1 and End_date is null and [Type] = 0 Order By Short_name").Result;
			if (mes.StatusCode == HttpStatusCode.OK)
			{
				string res = mes.Content.ReadAsStringAsync().Result;
				var json = JsonConvert.DeserializeObject<List<Subdivision>>(res);
				return json;
			}
			else
			{
				throw new Exception("Ошибка при работе с модулем WorkerService: не удалось выполнить запрос при попытке получить отдел НИ");
			}
		}
		/// <summary>
		/// Получить отдел по идентифкатору MSSQL
		/// </summary>
		/// <param name="sqlid">Идентификатор mssql</param>
		/// <returns>Отдел из MSSQL</returns>
		/// <exception cref="Exception">ошибка</exception>
		private static Subdivision GetSubdivision(int sqlid)
		{
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Add("X-EIPS-AuthToken", token);
			HttpResponseMessage mes = client.GetAsync(urlWorkerService + "Subdivision?filter=Id = " + sqlid.ToString()).Result;
			if (mes.StatusCode == HttpStatusCode.OK)
			{
				string res = mes.Content.ReadAsStringAsync().Result;
				var json = JsonConvert.DeserializeObject<List<Subdivision>>(res);
				return json.FirstOrDefault();
			}
			else
			{
				throw new Exception("Ошибка при работе с модулем WorkerService: не удалось выполнить запрос при попытке получить отдел НИ");
			}
		}
		#endregion
	}
}
