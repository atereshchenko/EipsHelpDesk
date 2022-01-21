using Dapper;
using Dapper.Contrib.Extensions;

using EipsHelpDesk.Entities;
using Npgsql;

using System;
using System.Linq;

namespace EipsHelpDesk.Services
{
    public interface IUserssettingsService
    {
        Userssettings GetByUserId(int userid);
        Userssettings Create(Userssettings userssettings);
    }

    public class UserssettingsService : IUserssettingsService
    {        
        string connection = null;        
        public static int countPage { get; set; }        
        public UserssettingsService(string connection)
        {
            this.connection = connection;
        }

        public Userssettings GetByUserId(int userid)
        {
            using (var db = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM userssettings where userid = " + userid.ToString() + ";";
                return db.Query<Userssettings>(query).FirstOrDefault();
            }
        }

        public Userssettings Create(Userssettings userssettings)
        {
            userssettings.createdon = DateTime.Now;
            userssettings.modifiedon = DateTime.Now;
            using (var db = new NpgsqlConnection(connection))
            {
                var userssettingsid = (int)db.Insert(userssettings);
                return GetByUserId(userssettingsid);
            }
        }
    }
}
