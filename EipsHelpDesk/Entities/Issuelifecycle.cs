using Dapper.Contrib.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EipsHelpDesk.Entities
{
	/// <summary>
	/// Жизненый цикл Обращения
	/// </summary>
	[Table("lifecycle")]
	public class Lifecycle
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
		/// Создатель
		/// </summary>
		public int createdbyid { get; set; }
		/// <summary>
		/// Дата изменения
		/// </summary>
		public DateTime modifiedon { get; set; }
		/// <summary>
		/// Изменил
		/// </summary>
		public int modifiedbyid { get; set; }
		/// <summary>
		/// Идентификатор обращения
		/// </summary>
		public int issueid { get; set; }
		/// <summary>
		/// Идентификатор статуса
		/// </summary>
		public int statusid { get; set; }
		/// <summary>
		/// Наименование статуса
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Ответственный сотрудник
		/// </summary>
		public int? userid { get; set; }
		/// <summary>
		/// Фактичческая дата
		/// </summary>
		public DateTime? datefact { get; set; }
	}
}
