using EipsHelpDesk.Entities;
using System.Collections.Generic;

namespace EipsHelpDesk.Models
{
    public class Response
    {
        /// <summary>
        /// Секция ответа Result при получении Worker-ов
        /// </summary>
        public data data { get; set; }
        /// <summary>
        /// Список отделов при получении Отделов НИ
        /// </summary>
        public IEnumerable<Subdivision> subdivisions { get; set; }
    }

    public class data
    {
        /// <summary>
        /// Список пользователей
        /// </summary>
        public IEnumerable<Workers> workers { get; set; }
    }
}
