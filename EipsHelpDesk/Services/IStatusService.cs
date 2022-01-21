using Dapper;
using EipsHelpDesk.Entities;
using Npgsql;

using System.Collections.Generic;
using System.Linq;

namespace EipsHelpDesk.Services
{
    /// <summary>
    /// Интерфейс для работы с Статусами обращений
    /// </summary>
    public interface IStatusService
    {
        /// <summary>
        /// Получить статус по ИД
        /// </summary>
        /// <param name="id">Идентификатор статуса</param>
        /// <returns>Статус</returns>
        Status GetById(int id);
        /// <summary>
        /// Получить полный список всех статусов
        /// </summary>
        /// <returns>полный список всех статусов</returns>
        IEnumerable<Status> GetList();

        IEnumerable<Status> GetListWithoutClosed();

    }
    /// <summary>
    /// Класс для работы с Статусами обращений
    /// </summary>
    public class StatusService : IStatusService
    {
        /// <summary>
        /// Строка подключения
        /// </summary>
        string connectionString = null;
        /// <summary>
        /// Кло-во страниц
        /// </summary>
        public static int countPage { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="conn">строка подключения</param>
        public StatusService(string conn)
        {
            connectionString = conn;
        }
        /// <summary>
        /// Получить статус по ИД
        /// </summary>
        /// <param name="id">Идентификатор статуса</param>
        /// <returns>Статус</returns>
        public Status GetById(int id)
        {
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM status Where id = " + id + ";";
                return db.Query<Status>(query).FirstOrDefault();
            }
        }

        public IEnumerable<Status> GetList()
        {
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM status;";
                return db.Query<Status>(query);
            }
        }

        public IEnumerable<Status> GetListWithoutClosed()
        {
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT * FROM status where id <> 5;";
                return db.Query<Status>(query);
            }
        }
    }
}
