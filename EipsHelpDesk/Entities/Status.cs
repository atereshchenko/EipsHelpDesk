using System;

namespace EipsHelpDesk.Entities
{
	public class Status
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int id { get; set; }
		/// <summary>
		/// Дата создания
		/// </summary>
		public DateTime createdon { get; set; }
		/// <summary>
		/// Кто создал
		/// </summary>
		public int createdbyid { get; set; }
		/// <summary>
		/// Дата изменения
		/// </summary>
		public DateTime modifiedon { get; set; }
		/// <summary>
		/// Кто изменил
		/// </summary>
		public int modifiedbyid { get; set; }
		/// <summary>
		/// Наименование
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Описание
		/// </summary>
		public string description { get; set; }
		/// <summary>
		/// Цветовое обозначение
		/// </summary>
		public string color { get; set; }
	}
}
