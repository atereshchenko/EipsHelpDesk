using Dapper;
using Dapper.Contrib.Extensions;
using EipsHelpDesk.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Services
{
	public interface ILifecycleService
    {
        IEnumerable<Lifecycle> GetByIssueId(int issueid);
        int Create(Lifecycle lifecycle);
        bool Update(Lifecycle lifecycle);
    }
	public class LifecycleService : ILifecycleService
    {
        /// <summary>
        /// Строка подключения
        /// </summary>
        string connection = null;
        /// <summary>
        /// Кло-во страниц
        /// </summary>
        public static int countPage { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="conn">строка подключения</param>
        public LifecycleService(string connection)
        {
            this.connection = connection;
        }

        public int Create(Lifecycle lifecycle)
        {            
            lifecycle.createdon = DateTime.Now;
            lifecycle.modifiedon = DateTime.Now;            

            using (var db = new NpgsqlConnection(connection))
            {
                var result = (int)db.Insert(lifecycle);
                return result;
            }
           
        }
        public IEnumerable<Lifecycle> GetByIssueId(int issueid)
        {
            using (var db = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM lifecycle Where issueid = " + issueid + " order by id;";
                return db.Query<Lifecycle>(query);
            }
        }

        public bool Update(Lifecycle lifecycle)
        {
            lifecycle.modifiedon = DateTime.Now;
            using (var db = new NpgsqlConnection(connection))
            {
                return db.Update<Lifecycle>(lifecycle);
            }
        }
    }
}
