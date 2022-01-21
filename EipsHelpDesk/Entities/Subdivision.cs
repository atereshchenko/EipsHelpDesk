namespace EipsHelpDesk.Entities
{
    public class Subdivision
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Наименование отдела
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Аббривиатура отдела
        /// </summary>
        public string short_name { get;set; }
        /// <summary>
        /// Родительский отдел
        /// </summary>
        public int id_parent { get; set; }
        /// <summary>
        /// Руководитель отдела
        /// </summary>
        public int id_boss { get; set; }
    }
}
