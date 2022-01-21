using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Entities
{
    [Table("sessions")]
    public class Sessions
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Дата создания записи
        /// </summary>
        public DateTime createdon { get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int userid { get; set; }
         /// <summary>
         /// логин пользователя
         /// </summary>
        public string login { get; set; }
        /// <summary>
        /// Действие
        /// </summary>
        public string action { get; set; }
        /// <summary>
        /// Результат действия
        /// </summary>
        public bool result { get; set; }
    }
}
