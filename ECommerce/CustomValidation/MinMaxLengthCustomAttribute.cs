using System.ComponentModel.DataAnnotations;

namespace ECommerce.CustomValidation
{
    public class MinMaxLengthCustomAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;
        public MinMaxLengthCustomAttribute(int minValue , int maxValue)
        {
            _minLength = minValue;
            _maxLength = maxValue;
        }
        public override bool IsValid(object? value)
        {
            if(value is string name)
            {
                if(name.Length >= _minLength && name.Length <= _maxLength)
                {
                    return true;
                }
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The Length of {name} must be between [{_minLength} ,{_maxLength}]";
        }
    }
}
