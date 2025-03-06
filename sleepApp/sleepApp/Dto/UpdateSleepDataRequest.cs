using sleepApp.Validation;
using System.ComponentModel.DataAnnotations;

namespace sleepApp.Dto
{
    public class UpdateSleepDataRequest
    {
        [Required(ErrorMessage = "Поле \"ID записи\" не должно быть пустым")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле \"Дата\" не должно быть пустым")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Поле \"ID респондента\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"ID респондента\" должно быть положительным")]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "Поле \"Время отхода ко сну\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время отхода ко сну\" должно быть положительным")]
        public double SleepStartTime { get; set; }

        [PositiveNumber(ErrorMessage = "Поле \"Время пробуждения\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Время пробуждения\" не должно быть пустым")]
        public double SleepEndTime { get; set; }

        [PositiveNumber(ErrorMessage = "Поле \"Общее время сна\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Общее время сна\" не должно быть пустым")]
        public double TotalSleepHours { get; set; }

        [Required(ErrorMessage = "Поле \"Качество сна\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка качества сна должна быть в диапазоне от 1 до 10")]
        public int SleepQuality { get; set; }

        [Required(ErrorMessage = "Поле \"Время для спорта\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время для спорта\" должно быть положительным")]
        public int ExerciseMinutes { get; set; }

        [PositiveNumber(ErrorMessage = "Поле \"Кофеин\" должно быть положительным")]
        [Required(ErrorMessage = "Поле \"Кофеин\" не должно быть пустым")]
        public int CaffeineIntakeMg { get; set; }

        [Required(ErrorMessage = "Поле \"Время у экрана\" не должно быть пустым")]
        [PositiveNumber(ErrorMessage = "Поле \"Время у экрана\" должно быть положительным")]
        public int ScreenTime { get; set; }

        [Required(ErrorMessage = "Поле \"Рабочее время\" не должно быть пустым")]
        public double WorkHours { get; set; }

        [Required(ErrorMessage = "Поле \"Производительность\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка производительности должна быть в диапазоне от 1 до 10")]
        public int ProductivityScore { get; set; }

        [Required(ErrorMessage = "Поле \"Настроение\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка настроения должна быть в диапазоне от 1 до 10")]
        public int MoodScore { get; set; }

        [Required(ErrorMessage = "Поле \"Уровень стресса\" не должно быть пустым")]
        [Range(1, 10, ErrorMessage = "Оценка уровня стресса должна быть в диапазоне от 1 до 10")]
        public int StressLevel { get; set; }

        public UpdateSleepDataRequest(int id,
            DateOnly date,
            int personId,
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
            this.Id = id;
            this.Date = date;
            this.PersonId = personId;
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

        public UpdateSleepDataRequest()
        { }

        public void Validate()
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