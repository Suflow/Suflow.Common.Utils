using System;
using System.ComponentModel.DataAnnotations;

namespace Suflow.Common.Utils
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MaxWordsAttribute : ValidationAttribute
    {
        private readonly int _maxWords;

        public MaxWordsAttribute(int maxWords):base("{0} has too many words.")
        {
            _maxWords = maxWords;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var valueAsString = value.ToString();
                if (valueAsString.Split(' ').Length > _maxWords)
                { 
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
