using System.ComponentModel.DataAnnotations;

namespace ECommerce.CustomValidation
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _fileSize;
        public MaxFileSizeAttribute(int fileSize)
        {
            _fileSize = fileSize;
        }
        public override bool IsValid(object? value)
        {
            if(value is IFormFile formFile)
            {
                if (formFile.Length <= _fileSize)
                {
                    return true;
                }
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The File Size must be less than or equal {_fileSize}";
        }
    }
}
