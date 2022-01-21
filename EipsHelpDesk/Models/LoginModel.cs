using System.ComponentModel.DataAnnotations;

namespace EipsHelpDesk.Models
{
	public class LoginModel
	{
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указан Email")]
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// URL переадресации после авторизации
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
