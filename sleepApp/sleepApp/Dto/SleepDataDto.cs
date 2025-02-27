using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sleepApp.Validation

namespace sleepApp.Dto
{
    public class SleepDataDto
    {
        public int Id { get; set; }

        private DateTime date { get; set; }
        [Required(ErrorMessage = "Поле \"ID респондента\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"ID респондента\" должно быть положительным")]
        private int PersonId { get; set; }
        [RegularExpression("^(2[0-4]|1[0-9]|[0-9])(\\.([0-5]?[0-9]))?$", ErrorMessage = "Неверный формат времени. Введите время через точку в формате HH.MM в 24 часовом формате")]
        [Required(ErrorMessage = "Поле \"Время отхода ко сну\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время отхода ко сну\" должно быть положительным")]
        private  double SleepStartTime { get; set; }
        [RegularExpression("^(2[0-4]|1[0-9]|[0-9])(\\.([0-5]?[0-9]))?$", ErrorMessage = "Неверный формат времени. Введите время через точку в формате HH.MM в 24 часовом формате")]
        [PositiveNumber(ErrorMessage = "Поле \"Время пробуждения\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Время пробуждения\" не должно быть пустым")]
        private double SleepEndTime { get; set; }
        [RegularExpression("^[0-9]*\\.?[0-9]*$", ErrorMessage = "Неверный формат времени. Введите время в формате HH.MM через точку")]
        [PositiveNumber(ErrorMessage = "Поле \"Общее время сна\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Общее время сна\" не должно быть пустым")]
        private double TotalSleepHours { get; set; }
        [Required(ErrorMessage = "Поле \"Качество сна\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка качества сна должна быть в диапазоне от 18 до 18 лет")]
        private int SleepQuality { get; set; }
        [Required(ErrorMessage = "Поле \"Время для спорта\" не должно быть пустым")]
        [Range(0, 360, ErrorMessage = "Время для спорта должно быть в диапазоне от 0 до 360 минут")]
        private int ExerciseMinutes { get; set; }
        [Range(0, 2000, ErrorMessage = "Количество кофеина должно быть в диапазоне от 0 до 2000 мг")]
        [Required(ErrorMessage = "Поле \"Кофеин\" не должно быть пустым")]
        private int CaffeineIntakeMg { get; set; }

        public SleepDataDto() {
        
        this.date = DateTime.Now;
                }
    }
}
