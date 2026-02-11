using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MVCLearningsPOC.Data.DataAnnotations
{
    public class NotFutureDateAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value,
                                                     ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            DateTime date = (DateTime)value;

            if (date > DateTime.Today)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        // Adds client-side validation rule
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-notfuturedate",
                                   ErrorMessage ?? "Future date not allowed");
        }
    }
}
