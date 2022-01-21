using System;

namespace EipsHelpDesk.Models
{
    /// <summary>
    /// Информация о пагинации
    /// </summary>
    public class PageViewModel
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber { get; private set; }
        /// <summary>
        /// Всего страниц
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Конструктор формирования пагинации
        /// </summary>
        /// <param name="count">кол-во записей</param>
        /// <param name="pageNumber">номер страницы</param>
        /// <param name="rows">кол-во отображаемых строк</param>
        public PageViewModel(int count, int pageNumber, int rows)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)rows);
        }

        /// <summary>
        /// Есть ли предыдущая страница
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return PageNumber > 1;
            }
        }
        /// <summary>
        /// Есть ли следующая страница
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return PageNumber < TotalPages;
            }
        }
    }
}
