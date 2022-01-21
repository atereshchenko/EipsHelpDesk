using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToHelpDescSync.SQL
{
	public class Workers
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int id { get; set; }
		/// <summary>
		/// Краткое имя
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Логин
		/// </summary>
		public string login { get; set; }
		/// <summary>
		/// Отдел
		/// </summary>
		public int id_department { get; set; }
		/// <summary>
		/// Табельный номер
		/// </summary>
		public string clock_number { get; set; }
		/// <summary>
		/// Должность
		/// </summary>
		public string job { get; set; }
		/// <summary>
		/// Дата увольнения
		/// </summary>
		public DateTime sacking_date { get; set; }
		/// <summary>
		/// Телефон сотрудника
		/// </summary>
		public string WorkPhone { get; set; }
	}
}
