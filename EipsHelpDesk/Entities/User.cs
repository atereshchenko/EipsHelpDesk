using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EipsHelpDesk.Entities
{
	/// <summary>
	/// Сотрудник
	/// </summary>
	[Table("Users")]
	public class User
	{
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
		/// <summary>
		/// Дата создания сотрудника
		/// </summary>
		public DateTime Createdon { get; set; }
		/// <summary>
		/// Дата изменения сотрудника
		/// </summary>
		public DateTime Modifiedon { get; set; }
		/// <summary>
		/// Имя сотрудника
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Логин сотрудника
		/// </summary>
		public string Login { get; set; }
		/// <summary>
		/// Должность сотрудника
		/// </summary>
		public string Positions { get; set; }
		/// <summary>
		/// Табельный номер сотрудника
		/// </summary>
		public string Personnelnumber { get; set; }

		/// <summary>
		/// Идентификатор отдела
		/// </summary>
		public int Departmentid { get; set; }

		#region Role - Роль сотрудника
		/// <summary>
		/// Идентификатор роли сотрудника
		/// </summary>
		public int? RoleId { get; set; }

		[Write(false)]
		[Computed]
		private Role role { get; set; }

		/// <summary>
		/// Роль сотрудника
		/// </summary>
		[Write(false)]
		[Computed]
		public Role Role
		{
			get
			{
				return role;
			}
			set
			{
				role = value;
				RoleId = value?.Id;
			}
		}
		#endregion
		
		[Write(false)]
		[Computed]
		public Userssettings Settings { get; set; }	

		public bool isactual { get; set; }
	}
}
