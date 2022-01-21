using System;
using System.Collections.Generic;
using System.Text;

namespace SqlToHelpDescSync.SQL
{
    public class Subdivision
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование отдела
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Аббривиатура отдела
        /// </summary>
        public string Short_name { get; set; }        
        /// <summary>
        /// Руководитель отдела
        /// </summary>
        public int Id_boss { get; set; }
    }
}
