using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToHelpDescSync.Postgres
{
	class User
	{
		/// <summary>
		/// Идентификатор (Postgres)
		/// </summary>
		public int id { get; set; }
		/// <summary>
		/// Дата создания (Postgres)
		/// </summary>
		public DateTime createdon { get; set; }
		/// <summary>
		/// Дата изменения (Postgres)
		/// </summary>
		public DateTime modifiedon { get; set; }
		/// <summary>
		/// SQL-идентификатор (Postgres)
		/// </summary>
		public int sqlid { get; set; }
		/// <summary>
		/// Идентификатор роли (Postgres)
		/// </summary>
		public int roleid { get; set; }
		/// <summary>
		/// Идентификатор отдела (Postgres)
		/// </summary>
		public int departmentid { get; set; }
		/// <summary>
		/// Наименование (Postgres)
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Табельный номер (Postgres)
		/// </summary>
		public string personnelnumber { get; set; }
		/// <summary>
		/// Логин (Postgres)
		/// </summary>
		public string login { get; set; }
		/// <summary>
		/// Должность (Postgres)
		/// </summary>
		public string positions { get; set; }
		/// <summary>
		/// Аткуальность (Postgres)
		/// </summary>
		public bool isactual { get; set; }
		/// <summary>
		/// Телефон сотрудника
		/// </summary>
		public string phone { get; set; }
	}
}
