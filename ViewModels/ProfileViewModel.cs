using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearningPlatform.ViewModels {
    public class ProfileViewModel {
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Дата регистрации")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Роли")]
        public string Roles { get; set; }
    }
}