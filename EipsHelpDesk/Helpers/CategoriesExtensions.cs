using Dapper;
using EipsHelpDesk.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Helpers
{
	public static class CategoriesExtensions
	{
		#region GetOwners - Кто ответственный
		public static IEnumerable<Categories> GetOwners(this IEnumerable<Categories> categories)
		{
			return categories.Select(x => x.GetOwner());
		}

		public static Categories GetOwner(this Categories category)
		{
			category.Owner = GetUser(category.ownerid);
			return category;
		}
		#endregion

		private static User GetUser(int id)
		{
			string query = "Select * from users Where id = " + id + ";";
			using (NpgsqlConnection db = new NpgsqlConnection(Startup.connection))
			{
				return db.Query<User>(query).FirstOrDefault();
			}
		}
	}
}
