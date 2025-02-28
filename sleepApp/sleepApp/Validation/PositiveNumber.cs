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
            if (value is int intValue)
            {
                if (intValue > 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? "Число должно быть положительным");
                }
            }
            else if (value is double doubleValue)
            {
                if (doubleValue > 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? "Число должно быть положительным");
                }
            }

            // Если тип данных не int или double
            return new ValidationResult("Недопустимый тип данных");
        }
    }
}
