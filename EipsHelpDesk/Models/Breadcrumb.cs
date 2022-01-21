namespace EipsHelpDesk.Models
{
    public class Breadcrumb
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отображаемый текст
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Контроллер
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// Метод контроллера
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// Роут-маршрутизация (controller/action/[id])
        /// </summary>
        public string Route { get; set; }
    }
}
