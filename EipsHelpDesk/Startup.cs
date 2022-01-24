using EipsHelpDesk.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Dapper;
using System.Text;

namespace EipsHelpDesk
{
	public class Startup
	{
		public static string host = Environment.GetEnvironmentVariable("PG_SERVER");
		public static string port = Environment.GetEnvironmentVariable("PG_PORT");
		public static string database = Environment.GetEnvironmentVariable("PG_DATABASE");
		public static string username = Environment.GetEnvironmentVariable("PG_USERNAME");
		public static string password = Environment.GetEnvironmentVariable("PG_PASSWORD");

		/// <summary>
		/// Строка подключения к БД
		/// </summary>
		public static string connection = "Host=" + host + ";Port=" + port + ";Database=" + database + ";Username=" + username + ";Password=" + password;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// установка конфигурации подключения
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
			{
				options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/account/login");
			});

			services.AddTransient<IUsersService, UsersService>(provider => new UsersService(connection));
			services.AddTransient<IIssueService, IssueService>(provider => new IssueService(connection));
			services.AddTransient<IStatusService, StatusService>(provider => new StatusService(connection));
			services.AddTransient<ICategoryService, CategoryService>(provider => new CategoryService(connection));
			services.AddTransient<ISessionService, SessionService>(provider => new SessionService(connection));
			services.AddTransient<IUserssettingsService, UserssettingsService>(provider => new UserssettingsService(connection));
			services.AddTransient<ILifecycleService, LifecycleService>(provider => new LifecycleService(connection));
			services.AddTransient<IReportService, ReportService>(provider => new ReportService(connection));

			services.AddDistributedMemoryCache();

			services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.Name = ".EipsHelpDesk.Session";
				options.Cookie.Expiration = TimeSpan.FromDays(1);
			});

			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.Use(async (context, next) =>
			{
				if (context.Request.Path.ToString().Contains("test") && (context.Request.Method == "GET"))
				{
					//context.Response.StatusCode = 200;
					//await context.Response.WriteAsync("Test OK!");
					if (GetConnectSQL())
					{
						context.Response.StatusCode = 200;
						await context.Response.WriteAsync("Test OK!");
					}
					else
					{
						context.Response.StatusCode = 500;
						await context.Response.WriteAsync("Test Errors!");
					}
					return;
				}
				await next.Invoke();
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");				
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();    // аутентификация
			app.UseAuthorization();     // авторизация

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			GetConnectSQL();
		}
		/// <summary>
		/// Проверка подключения к БД
		/// </summary>
		/// <returns></returns>
		public bool GetConnectSQL()
		{
			try
			{
				//Проверка подключения SQL
				using (var db = new NpgsqlConnection(connection))
				{
					string query = "SELECT version();";
					string res = db.ExecuteScalar<string>(query);
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($"The SQL Server Version: {res}");
					Console.ResetColor();					
					return true;
				}
			}
			catch (PostgresException ex)
			{
				//Создаем БД
				if (ex.SqlState == "3D000")
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"The specified database does not exist. Create a new db...");
					CreateBD();
					connection = "Server=" + host + ";Port=" + port + ";Database=" + database + ";Username=" + username + ";Password=" + password;
					Table table = new Table();					
					var result = table.CreateTables(connection);
					if (result)
					{
						result = table.FillInitialData(connection);
					}
					else
					{
						result = false;
					}
					return result;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"Postgres Exception Code: {ex.SqlState}");
					Console.WriteLine($"Test SQL connection test failed: {ex.MessageText} \n");
					Console.ResetColor();
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Test SQL connection test failed: {ex} \n");
				Console.ResetColor();
				return false;
			}
		}
		/// <summary>
		/// Мктод создания Базы данных
		/// </summary>
		/// <returns></returns>
		public bool CreateBD()
		{
			try
			{
				connection = "Server=" + host + ";Port=" + port + ";Username=" + username + ";Password=" + password + ";";
				var m_conn = new NpgsqlConnection(connection);
				var m_createdb_cmd = new NpgsqlCommand("CREATE DATABASE " + database + " WITH OWNER = postgres ENCODING = 'UTF8' LC_COLLATE = 'Russian_Russia.1251' LC_CTYPE = 'Russian_Russia.1251' TABLESPACE = pg_default CONNECTION LIMIT = -1;", m_conn);
				m_conn.Open();
				m_createdb_cmd.ExecuteNonQuery();
				m_conn.Close();
				
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Database {database} created successfully!");
				Console.ResetColor();
				return true;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}		
	}

	public class Table
	{
		/// <summary>
		/// Имя таблицы
		/// </summary>
		public  string name { get; set; }
		/// <summary>
		/// запрос для создания таблицы
		/// </summary>
		public  string query { get; set; }		
		/// <summary>
		/// Создание необходимых таблиц
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public bool CreateTables(string connection)
		{
			try
			{
				string issue = "create table issue (id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), createdon timestamp without time zone NOT NULL, createdbyid integer NOT NULL, modifiedon timestamp without time zone NOT NULL, modifiedbyid integer NOT NULL, subject text NOT NULL, text text NOT NULL, initiatorid integer NOT NULL, departmentid integer NOT NULL, categoryid integer NOT NULL, statusid integer NOT NULL, lifecycleid integer NOT NULL, planneddate timestamp without time zone, factdate timestamp without time zone, responsibleid integer, solution text, factdateclosed timestamp without time zone, CONSTRAINT pk_issue PRIMARY KEY (id) );";
				string departments = "create table departments(id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), createdon timestamp without time zone NOT NULL, modifiedon timestamp without time zone NOT NULL, name text NOT NULL, shortname text, userbossid integer, sqlid integer NOT NULL, sqlbossid integer, isactual boolean NOT NULL, CONSTRAINT pk_departments PRIMARY KEY (id));";
				string users = "create table users(id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),	createdon timestamp without time zone NOT NULL,	modifiedon  timestamp without time zone NOT NULL, name text NOT NULL, login text, positions text, personnelnumber text, departmentid integer NOT NULL, phone text, roleid integer, sqlid integer NOT NULL, isactual boolean NOT NULL, CONSTRAINT pk_users PRIMARY KEY (id));";
				string categories = "create table categories (id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), createdon timestamp without time zone NOT NULL, createdbyid integer NOT NULL, modifiedon timestamp without time zone NOT NULL, modifiedbyid integer NOT NULL, name text NOT NULL, description text, ownerid integer NOT NULL, CONSTRAINT pk_categories PRIMARY KEY (id));";
				string roles = "create table roles (id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), name text, CONSTRAINT pk_roles PRIMARY KEY (id));";
				string status = "create table status(id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), createdon timestamp without time zone NOT NULL, createdbyid integer NOT NULL, modifiedon timestamp without time zone NOT NULL, modifiedbyid integer NOT NULL, name text NOT NULL, color text, description text, CONSTRAINT pk_status PRIMARY KEY (id));";
				string lifecycle = "create table lifecycle (id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), createdon timestamp without time zone NOT NULL, createdbyid integer NOT NULL, modifiedon timestamp without time zone NOT NULL, modifiedbyid integer NOT NULL, issueid integer NOT NULL, statusid integer NOT NULL, name text NOT NULL, userid integer, datefact timestamp without time zone, CONSTRAINT pk_lifecycle PRIMARY KEY (id));";
				string sessions = "create table sessions (id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), createdon timestamp without time zone NOT NULL, userid integer NOT NULL, login text NOT NULL, action text NOT NULL, result boolean NOT NULL, CONSTRAINT pk_sessions PRIMARY KEY (id));";
				string userssettings = "create table userssettings(id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),createdon timestamp without time zone NOT NULL, modifiedon timestamp without time zone NOT NULL, userid integer,issueff integer,issuesf integer,issuetf integer,CONSTRAINT pk_userssettings PRIMARY KEY (id));";
								
				var m_conn = new NpgsqlConnection(connection);
				m_conn.Open();

				List<Table> sqlCommandCreate = new List<Table>();
				sqlCommandCreate.Add(new Table { name = "issue", query = issue });
				sqlCommandCreate.Add(new Table { name = "departments", query = departments });
				sqlCommandCreate.Add(new Table { name = "roles", query = roles });
				sqlCommandCreate.Add(new Table { name = "users", query = users });
				sqlCommandCreate.Add(new Table { name = "categories", query = categories });
				sqlCommandCreate.Add(new Table { name = "status", query = status });
				sqlCommandCreate.Add(new Table { name = "lifecycle", query = lifecycle });
				sqlCommandCreate.Add(new Table { name = "sessions", query = sessions });
				sqlCommandCreate.Add(new Table { name = "userssettings", query = userssettings });
				
				Console.ForegroundColor = ConsoleColor.Red;
				foreach (var _command in sqlCommandCreate)
				{
					var command = new NpgsqlCommand(_command.query, m_conn);
					command.ExecuteNonQuery();
					Console.WriteLine($"Create table: {_command.name}");
				}
				m_conn.Close();
				Console.ResetColor();
				return true;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		/// <summary>
		/// Заполнение данных певроначальными данными
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public bool FillInitialData(string connection)
		{
			string insertroles1 = "INSERT INTO public.roles(name) VALUES ('Administrators');";
			string insertroles2 = "INSERT INTO public.roles(name) VALUES ('Moderators');";
			string insertroles3 = "INSERT INTO public.roles(name) VALUES ('Users');";

			string insertusers1 = $"INSERT INTO public.users(createdon, modifiedon, name, login, positions, personnelnumber, departmentid, phone, roleid, sqlid, isactual)VALUES('{DateTime.Now}', '{DateTime.Now}', 'Администратор', 'administrator', 'администратор модуля', '00000000', 0, '0', 1, 0, '1'); ";

			string insertstatus1 = $"INSERT INTO status(createdon, createdbyid, modifiedon, modifiedbyid, name, description, color) VALUES('{DateTime.Now}', 1, '{DateTime.Now}', 1, 'New', 'Автоматически присваивается при регистрации заявки', 'primary'); ";
			string insertstatus2 = $"INSERT INTO public.status(createdon, createdbyid, modifiedon, modifiedbyid, name, description, color) VALUES('{DateTime.Now}', 1, '{DateTime.Now}', 1, 'In Progress', 'Сотрудники компании работают над заявкой', 'info'); ";			
			string insertstatus3 = $"INSERT INTO public.status(createdon, createdbyid, modifiedon, modifiedbyid, name, description, color) VALUES('{DateTime.Now}', 1, '{DateTime.Now}', 1, 'Fixed', 'Заявка отработана и заказчик подтверждает решение вопроса', 'success'); ";
			string insertstatus4 = $"INSERT INTO public.status(createdon, createdbyid, modifiedon, modifiedbyid, name, description, color) VALUES('{DateTime.Now}', 1, '{DateTime.Now}', 1, 'Canceled', 'Заявка открыта ошибочно или приостановлена по требованию инициатора', 'danger'); ";
			string insertstatus5 = $"INSERT INTO public.status(createdon, createdbyid, modifiedon, modifiedbyid, name, description, color) VALUES('{DateTime.Now}', 1, '{DateTime.Now}', 1, 'Closed', 'Вопрос по заявке полностью решен и заявка закрыта', 'secondary'); ";
			
			var m_conn = new NpgsqlConnection(connection);
			m_conn.Open();

			List<Table> sqlinsert = new List<Table>();
			sqlinsert.Add(new Table { name = "roles", query = insertroles1 });
			sqlinsert.Add(new Table { name = "roles", query = insertroles2 });
			sqlinsert.Add(new Table { name = "roles", query = insertroles3 });
			sqlinsert.Add(new Table { name = "users", query = insertusers1 });
			sqlinsert.Add(new Table { name = "status", query = insertstatus1 });
			sqlinsert.Add(new Table { name = "status", query = insertstatus2 });
			sqlinsert.Add(new Table { name = "status", query = insertstatus3 });
			sqlinsert.Add(new Table { name = "status", query = insertstatus4 });
			sqlinsert.Add(new Table { name = "status", query = insertstatus5 });

			Console.ForegroundColor = ConsoleColor.Red;
			foreach (var _command in sqlinsert)
			{
				var command = new NpgsqlCommand(_command.query, m_conn);
				command.ExecuteNonQuery();
				Console.WriteLine($"data added in table: {_command.name}");
			}
			m_conn.Close();
			Console.ResetColor();
			return true;
		}
	}
}
