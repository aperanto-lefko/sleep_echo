using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sleepApp.Dto
{
    class RespondentDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле \"Имя\" не должно быть пустым")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Имя должно содержать только латинские буквы")]
        [StringLength(20, ErrorMessage = "Имя не должно превышать 20 символов")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Поле \"Фамилия\" не должно быть пустым")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Фамилия должна содержать только латинские буквы")]
        [StringLength(30, ErrorMessage = "Фамилия не должна превышать 30 символов")]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Поле \"Пол\" не должно быть пустым")]
        [StringLength(10, ErrorMessage = "Пол не должен превышать 10 символов")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Поле \"Страна\" не должно быть пустым")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Название страны должно содержать только латинские буквы")]
        [StringLength(20, ErrorMessage = "Название страны не должно превышать 20 символов")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Поле \"Возраст\" не должно быть пустым")]
        [Range(18, 80, ErrorMessage = "Возраст должен быть в диапазоне от 18 до 18 лет")]
        public int Age { get; set; }

        public RespondentDto(string firsName, string lastName, string email, string gender, string country, int age)
        {
            this.FirstName = firsName;
            this.LastName = lastName;
            this.Email = email;
            this.Gender = gender;
            this.Country = country;
            this.Age = age;
            Validate();
        }

        public override string ToString()
        {
            return $"ID: {Id}, first name: {FirstName}, last name: {LastName}, age: {Age},  email: {Email}, gender: {Gender}, country: {Country}";
        }

        private void Validate()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(this, context, results, true);
            if (!isValid)
            {
                var errorMessages = results.Select(r => r.ErrorMessage);
                throw new ValidationException(string.Join("\n", errorMessages));
            }

        }
    }
}
