namespace EipsHelpDesk.Entities
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
	}
}
