using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Entities
{
    /// <summary>
    /// Настройки пользователя
    /// </summary>
    [Table("userssettings")]
    public class Userssettings
    {
        public int id { get; set; }
        public DateTime createdon { get; set; }
        public DateTime modifiedon { get; set; }
        public int userid { get; set; }
        public int issueff { get; set; }
        public int issuesf { get; set; }
        public int issuetf { get; set; }
    }
}
