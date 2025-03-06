using System.ComponentModel.DataAnnotations;

namespace sleepApp.Validation
{
    public class PositiveNumber : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return value switch
            {
                int intValue when intValue >= 0 => ValidationResult.Success,
                double doubleValue when doubleValue >= 0 => ValidationResult.Success,
                int or double => new ValidationResult(ErrorMessage ?? "Число должно быть положительным"), //Обрабатывает случай, когда значение является int или double, но не удовлетворяет условию > 0.
                _ => new ValidationResult("Недопустимый тип данных")
            };
        }
    }
}