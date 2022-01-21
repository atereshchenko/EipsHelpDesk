using Dapper;
using Dapper.Contrib.Extensions;

using EipsHelpDesk.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EipsHelpDesk.Services
{
    /// <summary>
    /// Интерфейс для работы с Обращениями
    /// </summary>
    public interface IIssueService
    {        
        IEnumerable<Issue> ToList();
        IEnumerable<Issue> ToList(int page, int rows);
        IEnumerable<Issue> ToList(int page, int rows, string filter);
        IEnumerable<Issue> GetByInitiator(int initiatorid);
        IEnumerable<Issue> GetByResponsible(int responsibleid);
        public int Count();
        public int Count(string filter);
        Issue GetById(int id);        
        int Create(Issue issue);        
        bool Update(Issue issue);
    }
    /// <summary>
    /// Класс для работы с Обращениями
    /// </summary>
    public class IssueService: IIssueService
    {
        /// <summary>
        /// Строка подлкючения
        /// </summary>
        string connectionString = null;
        /// <summary>
        /// Кол-во строк
        /// </summary>
        public static int countPage { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="conn">строка подключения</param>
        public IssueService(string conn)
        {
            connectionString = conn;
        }       
        public IEnumerable<Issue> ToList()
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM issue ORDER BY id DESC;";
                return db.Query<Issue>(query);
            }
        }        
        public IEnumerable<Issue> ToList(int page, int rows)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                int countRecords = Count();
                countPage = (int)Math.Ceiling(countRecords / (double)rows);
                int _page = (page - 1) * rows;
                string query = "SELECT * FROM issue ORDER BY id DESC OFFSET " + _page.ToString() + " ROWS FETCH NEXT " + rows.ToString() + " ROWS ONLY;";

                var tmp = db.Query<Issue>(query);
                return tmp;
            }
        }
        public IEnumerable<Issue> ToList(int page, int rows, string filter)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                int countRecords = Count();
                countPage = (int)Math.Ceiling(countRecords / (double)rows);
                int _page = (page - 1) * rows;
                string query = "SELECT * FROM issue " + filter + " OFFSET " + _page.ToString() + " ROWS FETCH NEXT " + rows.ToString() + " ROWS ONLY;";
                var tmp = db.Query<Issue>(query);
                return tmp;
            }
        }
        public IEnumerable<Issue> MyIssueList(int page, int rows, int userid)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                int countRecords = MyIssueCount(userid);
                countPage = (int)Math.Ceiling(countRecords / (double)rows);
                int _page = (page - 1) * rows;
                string query = "SELECT * FROM issue Where initiatorid = " + userid.ToString() + " ORDER BY id DESC OFFSET " + _page.ToString() + " ROWS FETCH NEXT " + rows.ToString() + " ROWS ONLY;";

                var tmp = db.Query<Issue>(query);
                return tmp;
            }
        }
        public int MyIssueCount(int userid)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                string countquery = "SELECT COUNT(*) FROM issue where initiatorid = " + userid.ToString() + "; ";
                return db.ExecuteScalar<int>(countquery);
            }
        }
        public IEnumerable<Issue> GetByInitiator(int initiatorid)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {                
                string query = "SELECT * FROM issue where initiatorid = "+ initiatorid + ";";
                var tmp = db.Query<Issue>(query);
                return tmp;
            }
        }
        public IEnumerable<Issue> GetByResponsible(int responsibleid)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM issue where responsibleid = " + responsibleid + ";";
                var tmp = db.Query<Issue>(query);
                return tmp;
            }
        }
        public int Count()
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                string countquery = "SELECT COUNT(*) FROM issue;";
                return db.ExecuteScalar<int>(countquery);
            }
        }
        public int Count(string filter)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                string countquery = "SELECT COUNT(*) FROM issue " + filter;
                return db.ExecuteScalar<int>(countquery);
            }
        }
        public Issue GetById(int id)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM issue Where id = " + id + ";";
                return db.Query<Issue>(query).FirstOrDefault();
            }
        }        
		public int Create(Issue issue)
		{
			issue.createdon = DateTime.Now;
			issue.modifiedon = DateTime.Now;
			using (var db = new NpgsqlConnection(connectionString))
			{
                return (int)db.Insert(issue);
			}
		}
        public bool Update(Issue issue)
        {
            using (var db = new NpgsqlConnection(connectionString))
            {
                issue.modifiedon = DateTime.Now;
                return db.Update<Issue>(issue);                
            }
        }
    }
}
