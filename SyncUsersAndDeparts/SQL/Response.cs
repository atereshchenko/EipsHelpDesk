using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToHelpDescSync.SQL
{
	public class Response
	{
		/// <summary>
		/// Секция ответа Result при получении Worker-ов
		/// </summary>
		public data data { get; set; }
	}
	public class data
	{
		/// <summary>
		/// Список пользователей
		/// </summary>
		public IEnumerable<Workers> workers { get; set; }
	}
}
