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
    public interface ISessionService
    {
        int Create(Sessions session);
        IEnumerable<Sessions> GetList(int page, int rows);
        int Count();
    }
    public class SessionService : ISessionService
    {
        /// <summary>
        /// Строка подлкючения
        /// </summary>
        string connection = null;
        /// <summary>
        /// Кол-во строк
        /// </summary>
        public static int countPage { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="connection">строка подключения</param>
        public SessionService(string connection)
        {
            this.connection = connection;
        }

        public int Create(Sessions session)
        {
            session.createdon = DateTime.Now;
            using (var db = new NpgsqlConnection(connection))
            {
                return (int)db.Insert(session);                
            }
        }

        /// <summary>
        /// Получить коллекцию Ссесий
        /// </summary>
        /// <returns>Коллекция ссесий</returns>
        public IEnumerable<Sessions> GetList(int page, int rows)
        {
            using (var db = new NpgsqlConnection(connection))
            {
                int countRecords = Count();
                countPage = (int)Math.Ceiling(countRecords / (double)rows);
                int _page = (page - 1) * rows;
                string query = "SELECT * FROM sessions ORDER BY id DESC OFFSET " + _page.ToString() + " ROWS FETCH NEXT " + rows.ToString() + " ROWS ONLY;";

                var tmp = db.Query<Sessions>(query);
                return tmp;
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
                string countquery = "SELECT COUNT(*) FROM sessions;";
                return db.ExecuteScalar<int>(countquery);
            }
        }
    }
}
