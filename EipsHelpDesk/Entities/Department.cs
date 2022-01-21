using Dapper.Contrib.Extensions;

using System;

namespace EipsHelpDesk.Entities
{
	/// <summary>
	/// Отдел сотрудника
	/// </summary>
	[Table("Departments")]
	public class Department
	{
		/// <summary>
		/// Идентификатор отдела
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Дата создания отдела
		/// </summary>
		public DateTime Createdon { get; set; }
		/// <summary>
		/// Дата изменения отдела
		/// </summary>
		public DateTime Modifiedon { get; set; }
		/// <summary>
		/// Наименование отдела
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Кратко наименование отдела
		/// </summary>
		public string Shortname { get; set; }
		/// <summary>
		/// Руководитель отдела
		/// </summary>
		public int Userbossid { get; set; }
		/// <summary>
		/// Актуальность отдела
		/// </summary>
		public bool Isactual { get; set; }

	}
}
