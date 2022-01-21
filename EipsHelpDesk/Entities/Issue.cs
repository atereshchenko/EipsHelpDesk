using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace EipsHelpDesk.Entities
{
    /// <summary>
    /// Обращения
    /// </summary>
    [Table("issue")]
    public class Issue
    {
        /// <summary>
        /// Идентификатор обращения
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime createdon { get; set; }

        #region Created - создал
        /// <summary>
        /// Идентификатор, кто создал
        /// </summary>
        public int createdbyid { get; set; }

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
                createdbyid = value.Id;
            }
        }
		#endregion

		/// <summary>
		/// Дата изменения
		/// </summary>
		public DateTime modifiedon { get; set; }

        #region Modified - изменил
        /// <summary>
        /// Идентификатор, кто изменил
        /// </summary>
        public int modifiedbyid { get; set; }

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
                modifiedbyid = value.Id;
            }
        }
		#endregion

		/// <summary>
		/// Тема замечания
		/// </summary>
		public string subject { get; set; }

        /// <summary>
        /// Текст замечания
        /// </summary>
        public string text { get; set; }

        #region Initiator - инициатор
        /// <summary>
        /// Идентификатор инициатора обращения (у кого возникла проблема)
        /// </summary>
        public int initiatorid { get; set; }

        [Write(false)]
        [Computed]
        private User initiator { get; set; }
        /// <summary>
        /// Инициатор обращения (у кого возникла проблема)
        /// </summary>
        [Write(false)]
        [Computed]
        public User Initiator
        {
            get
            {
                return initiator;
            }
            set
            {
                initiator = value;
                initiatorid = value.Id;
            }
        }
        #endregion

        #region Responsible - ответственный за решение сотрудник
        /// <summary>
        /// Идентификатор ответственного за решение обращения сотрудника 
        /// </summary>
        public int? responsibleid { get; set; }

        [Write(false)]
        [Computed]
        private User responsible { get; set; }
        /// <summary>
        /// Ответственный за решение обращения сотрудник 
        /// </summary>
        [Write(false)]
        [Computed]
        public User Responsible
        {
            get
            {
                return responsible;
            }
            set
            {
                responsible = value;
                responsibleid = value?.Id;
            }
        }
        #endregion

        #region Department - Отдел инициатора
        /// <summary>
        /// Идентификатор отдела инициатора
        /// </summary>
        public int? departmentid { get; set; }

        [Write(false)]
        [Computed]
        private Department department { get; set; }
        /// <summary>
        /// Отдел инициатора
        /// </summary>
        [Write(false)]
        [Computed]
        public Department Department
        {
            get
            {
                return department;
            }
            set
            {
                department = value;
                departmentid = value.Id;
            }
        }
		#endregion

		#region Category - категория обращения
		/// <summary>
		/// Идентификатор категории обращения
		/// </summary>
		public int categoryid { get; set; }
        [Write(false)]
        [Computed]
        private Categories сategory { get; set; }
        
        /// <summary>
        /// Категория обращения
        /// </summary>
        [Write(false)]
        [Computed]
        public Categories Category
        {
            get
            {
                return сategory;
            }
            set
            {
                сategory = value;
                categoryid = value.id;
            }
        }
        #endregion
        
		/// <summary>
		/// Плановая дата решения проблемы
		/// </summary>
		public DateTime? planneddate { get; set; }
        
        /// <summary>
        /// Фактическая дата решения проблемы
        /// </summary>
        public DateTime? factdate { get; set; }

        #region Status - статус обращения
        /// <summary>
        /// Текущий статус обращения
        /// </summary>
        public int statusid { get; set; }

        [Write(false)]
        [Computed]
        private Status status { get; set; }
        /// <summary>
        /// Текущий статус обращения
        /// </summary>
        [Write(false)]
        [Computed]
        public Status Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                statusid = value.id;
            }
        }
        #endregion


        #region Lifecycle - Жизненный цикл обращения
        public int lifecycleid { get; set; }

        [Write(false)]
        [Computed]
        private Lifecycle lifecycle { get; set; }
        /// <summary>
        /// Текущий статус обращения
        /// </summary>
        [Write(false)]
        [Computed]
        public Lifecycle Lifecycle
        {
            get
            {
                return lifecycle;
            }
            set
            {
                lifecycle = value;
                lifecycleid = value.id;
            }
        }
        #endregion

        /// <summary>
        /// Решение
        /// </summary>
        //[Required(ErrorMessage = "Не указан текст решения")]
        public string solution { get; set; }

		/// <summary>
		/// Принять/отклонить
		/// </summary>
		[Write(false)]
		[Computed]
		public bool isaccept { get; set; }
        /// <summary>
        /// Фактическая дата перехода обращения в состояние Closed
        /// </summary>
        public DateTime? factdateclosed { get; set; }
    }
}
