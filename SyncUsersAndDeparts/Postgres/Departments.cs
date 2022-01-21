using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToHelpDescSync.Postgres
{
	class Department
	{
		/// <summary>
		/// Идентификатор отдела (Postgres)
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
		/// Идентификатор руководителя (Postgres)
		/// </summary>
		public int userbossid { get; set; }
		/// <summary>
		/// Ключ синхронизации отдела с SQL (Postgres)
		/// </summary>
		public int sqlid { get; set; }
		/// <summary>
		/// Ключ синхронизации руководителя отдела с SQL (Postgres)
		/// </summary>
		public int sqlbossid { get; set; }
		/// <summary>
		/// Наименование отдела (Postgres)
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Краткое наименование отдела (Postgres)
		/// </summary>
		public string shortname { get; set; }
		/// <summary>
		/// Аткуальность отдела (Postgres)
		/// </summary>
		public bool isactual { get; set; }
	}
}
