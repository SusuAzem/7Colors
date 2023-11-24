using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel.DataAnnotations;

namespace _7Colors.Utility
{
    public class NotEqualAttribute : ValidationAttribute
    {
        public readonly string OtherProperty;
        public readonly string OtherPropertyName;

        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of the {1}.";
        
        private readonly string errorMsg;

        public NotEqualAttribute(string otherProperty, string otherPropertyName, string errorMsg)
            : base(DefaultErrorMessage)
        {
            OtherProperty = otherProperty;
            OtherPropertyName = otherPropertyName;
            this.errorMsg = errorMsg;
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            //if (value != null)
            
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty!.GetValue(validationContext.ObjectInstance, null);

                if (value!.Equals(otherPropertyValue))
                {
                    return new ValidationResult(errorMsg);
                }
                else
                    return ValidationResult.Success!;
            
        }

    }
}    

