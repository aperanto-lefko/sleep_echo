using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sleepApp.Model;
using sleepApp.Validation;

namespace sleepApp.Dto
{
    public class SleepDataDto
    {
        public int Id { get; set; }

        private DateTime Date { get; set; }
        [Required(ErrorMessage = "Поле \"ID респондента\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"ID респондента\" должно быть положительным")]
        private int PersonId { get; set; }
        [RegularExpression("^(2[0-4]|1[0-9]|[0-9])(\\.([0-5]?[0-9]))?$", ErrorMessage = "Неверный формат времени. Введите время через точку в формате HH.MM в 24 часовом формате")]
        [Required(ErrorMessage = "Поле \"Время отхода ко сну\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время отхода ко сну\" должно быть положительным")]
        private double SleepStartTime { get; set; }
        [RegularExpression("^(2[0-4]|1[0-9]|[0-9])(\\.([0-5]?[0-9]))?$", ErrorMessage = "Неверный формат времени. Введите время через точку в формате HH.MM в 24 часовом формате")]
        [PositiveNumber(ErrorMessage = "Поле \"Время пробуждения\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Время пробуждения\" не должно быть пустым")]
        private double SleepEndTime { get; set; }
        [RegularExpression("^[0-9]*\\.?[0-9]*$", ErrorMessage = "Неверный формат времени. Введите время в формате HH.MM через точку или целое")]
        [PositiveNumber(ErrorMessage = "Поле \"Общее время сна\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Общее время сна\" не должно быть пустым")]
        private double TotalSleepHours { get; set; }
        [Required(ErrorMessage = "Поле \"Качество сна\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка качества сна должна быть в диапазоне от 1 до 10")]
        private int SleepQuality { get; set; }
        [Required(ErrorMessage = "Поле \"Время для спорта\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время для спорта\" должно быть положительным")]
        private int ExerciseMinutes { get; set; }
        [PositiveNumber(ErrorMessage = "Поле \"Кофеин\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Кофеин\" не должно быть пустым")]
        private int CaffeineIntakeMg { get; set; }
        [Required(ErrorMessage = "Поле \"Время у экрана\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время у экрана\" должно быть положительным")]
        private int ScreenTime { get; set; }
        [Required(ErrorMessage = "Поле \"Рабочее время\" не должно быть пустым")]
        [RegularExpression("^[0-9]*\\.?[0-9]*$", ErrorMessage = "Неверный формат времени. Введите время в формате HH.MM через точку или целое")]
        private double WorkHours { get; set; }
        [Required(ErrorMessage = "Поле \"Производительность\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка производительности должна быть в диапазоне от 1 до 10")]
        private int ProductivityScore { get; set; }
        [Required(ErrorMessage = "Поле \"Настроение\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка настроения должна быть в диапазоне от 1 до 10")]
        private int MoodScore { get; set; }
        [Required(ErrorMessage = "Поле \"Уровень стресса\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка уровня стресса должна быть в диапазоне от 1 до 10")]
        private int StressLevel { get; set; }
        public SleepDataDto(int personId,
            double sleepStartTime,
            double sleepEndTime,
            double totalSleepHours,
            int sleepQuality,
            int exerciseMinutes,
            int caffeineIntakeMg,
            int screenTime,
            double workHours,
            int productivityScore,
            int moodScore,
            int stressLevel)
        {

            this.Date = DateTime.Now.Date;
            this.SleepStartTime = sleepStartTime;
            this.SleepEndTime = sleepEndTime;
            this.TotalSleepHours = totalSleepHours;
            this.SleepQuality = sleepQuality;
            this.ExerciseMinutes = exerciseMinutes;
            this.CaffeineIntakeMg = caffeineIntakeMg;
            this.ScreenTime = screenTime;
            this.WorkHours = workHours;
            this.ProductivityScore = productivityScore;
            this.MoodScore = moodScore;
            this.StressLevel = stressLevel;
            Validate();
        }

        public SleepDataDto() { }

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
        public override string ToString()
        {
            return $"ID: {Id} для respondent ID: {PersonId}";
        }
    }
}
