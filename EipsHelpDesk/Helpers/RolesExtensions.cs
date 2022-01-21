using Dapper;
using EipsHelpDesk.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Helpers
{
    public static class RolesExtensions
    {
		public static IEnumerable<User> GetAdminUsers(this IEnumerable<User> users)
		{
			return users.Select(x => x.GetAdminUser());
		}

		public static User GetAdminUser(this User user)
		{
			string queryGetUser = "SELECT * FROM users where workerId = " + user.Id.ToString()+";";			

			using (NpgsqlConnection db = new NpgsqlConnection(Startup.connection))
			{
				var _user = db.Query<User>(queryGetUser).FirstOrDefault();
				if (_user != null)
				{
					user.RoleId = _user.RoleId;

					string queryGetRole = "SELECT * FROM roles where id = " + user.RoleId.ToString() + ";";
					var _role = db.Query<Role>(queryGetRole).FirstOrDefault();
					user.Role = _role;
				}
				else
				{
					Role role = new Role
					{
						Id = 0,
						Name = "Users"
					};
					user.Role = role;
				}
			}			
			return user;
		}
	}
}
