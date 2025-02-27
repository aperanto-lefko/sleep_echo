using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sleepApp.Validation
{
    public class PositiveNumber:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return value switch
            {
                int intValue when intValue > 0 => ValidationResult.Success,
                int _ => new ValidationResult(ErrorMessage ?? "Число должно быть положительным"),
                double doubleValue when doubleValue > 0 => ValidationResult.Success,
                double _ => new ValidationResult(ErrorMessage ??  "Число должно быть положительным"),
                _ => new ValidationResult("Недопустимый тип данных")
            };
           }
    }
}
