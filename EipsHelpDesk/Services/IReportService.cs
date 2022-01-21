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
    public interface IReportService
    {
        IEnumerable<Responsible> GetResponsibles();
    }

    public class ReportService : IReportService
    {
        string connection = null;
        public ReportService(string connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Responsible> GetResponsibles()
        {
            using (var db = new NpgsqlConnection(connection))
            {
                //string query = "select distinct responsibleid, users.name from issue as issue inner join users as users on users.id = issue.responsibleid order by users.name;";
                string query = "select distinct issue.responsibleid, users.name, coalesce(stnew.countst, 0) as stnew, coalesce(stprogres.countst, 0) as stprogres, coalesce(stfixed.countst, 0) as stfixed," +
                    " coalesce(stcencel.countst, 0) as stcencel, coalesce(stclose.countst, 0) as stclose " +
                    " from issue as issue " +
                    " inner join users as users on users.id = issue.responsibleid " +
                    " left join (select responsibleid, count(*) as countst from issue where statusid = 1 group by responsibleid) as stnew on issue.responsibleid = stnew.responsibleid " +
                    " left join (select responsibleid, count(*) as countst from issue where statusid = 2 group by responsibleid) as stprogres on issue.responsibleid = stprogres.responsibleid " +
                    " left join (select responsibleid, count(*) as countst from issue where statusid = 3 group by responsibleid) as stfixed on issue.responsibleid = stfixed.responsibleid " +
                    " left join (select responsibleid, count(*) as countst from issue where statusid = 4 group by responsibleid) as stcencel on issue.responsibleid = stcencel.responsibleid " +
                    " left join (select responsibleid, count(*) as countst from issue where statusid = 5 group by responsibleid) as stclose on issue.responsibleid = stclose.responsibleid " +
                    " order by users.name;";
                return db.Query<Responsible>(query);
            }
        }
    }
    
    public class Responsible
    {
        /// <summary>
        /// идентификатор
        /// </summary>
        public int responsibleid { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string name { get; set; }
        public int stnew { get; set; }
        public int stprogres { get; set; }
        public int stfixed { get; set; }
        public int stcencel { get; set; }
        public int stclose { get; set; }
    }    
}
