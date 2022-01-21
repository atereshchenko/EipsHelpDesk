using Dapper.Contrib.Extensions;
using System;

namespace EipsHelpDesk.Entities
{
    [Table("categories")]
    public class Categories
    {
		/// <summary>
		/// Идентификатор обращения
		/// </summary>
		public int id { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime createdon { get; set; }

        #region Created
        /// <summary>
        /// Идентификатор, кто создал
        /// </summary>
        public int createdById { get; set; }

        [Write(false)]
        [Computed]
        private User created { get; set; }
        /// <summary>
        /// Создатель
        /// </summary>
        [Write(false)]
        [Computed]
        public User Created
        {
            get
            {
                return created;
            }
            set
            {
                created = value;
                createdById = value.Id;
            }
        }
        #endregion

        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime modifiedon { get; set; }

        #region Modified
        /// <summary>
        /// Идентификатор, кто изменил
        /// </summary>
        public int modifiedById { get; set; }

        [Write(false)]
        [Computed]
        private User modified { get; set; }
        /// <summary>
        /// Кто изменил
        /// </summary>
        [Write(false)]
        [Computed]
        public User Modified
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
                modifiedById = value.Id;
            }
        }
        #endregion

        /// <summary>
        /// Наименование категории
        /// </summary>
        public string Name { get; set; }
        public string description { get; set; }

		#region Owner
        /// <summary>
        /// Идентификатор ответственного
        /// </summary>
		public int ownerid { get; set; }

        [Write(false)]
        [Computed]
        private User owner { get; set; }
        /// <summary>
        /// Ответственный
        /// </summary>
        [Write(false)]
        [Computed]
        public User Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
                ownerid = value.Id;
            }
        }
		#endregion
	}
}
