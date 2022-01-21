using Dapper;
using EipsHelpDesk.Entities;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace EipsHelpDesk.Services
{
    /// <summary>
    /// Интерфейс для работы с категориями обращений
    /// </summary>
    public interface ICategoryService
    {
        IEnumerable<Categories> GetList();
        Categories GetById(int id);
		//int? GetById(int? categoryid);
	}
    public class CategoryService : ICategoryService
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
        /// <param name="conn">строка подключения</param>
        public CategoryService(string connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Categories> GetList()
        {
            using (var db = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM categories ORDER BY id DESC;";
                return db.Query<Categories>(query);
            }            
        }

        public Categories GetById(int id)
        {
            using (var db = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM categories Where id = " + id + ";";
                return db.Query<Categories>(query).FirstOrDefault();
            }
        }
    }
}
