using Dapper;
using EipsHelpDesk.Entities;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace EipsHelpDesk.Helpers
{
	public static class UserExtensions
	{
		#region GetRole - Определение роли пользователя
		public static IEnumerable<User> GetRoles(this IEnumerable<User> users)
		{
			return users.Select(x => x.GetRole());
		}

		public static User GetRole(this User user)
		{
			user.Role = Role(user.RoleId);
			return user;
		}
		#endregion

		#region GetSettings - парамтеры настроек пользователя
		//public static IEnumerable<User> GetSettings(this IEnumerable<User> users)
		//{
		//	return users.Select(x => x.GetSetting());
		//}
		public static User GetSetting(this User user)
		{
			user.Settings = GetUserSetting(user.Id);
			return user;
		}

		#endregion

		#region Privet method

		public static Userssettings GetUserSetting(int id)
		{
			using (var db = new NpgsqlConnection(Startup.connection))
			{
				string query = "select * from userssettings where userid =  " + id.ToString() + ";";
				return db.Query<Userssettings>(query).FirstOrDefault();
			}
		}

		public static Role Role(int? id)
		{
			using (var db = new NpgsqlConnection(Startup.connection))
			{
				string query = "select * from roles where id = " + id.ToString() + ";";
				return db.Query<Role>(query).FirstOrDefault();
			}
		}
		#endregion
	}
}
