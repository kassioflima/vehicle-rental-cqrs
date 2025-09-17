using System.ComponentModel.DataAnnotations;

namespace vehicle_rental.api.Validators;

public class AllowedFileExtensionsAttribute(params string[] extensions) : ValidationAttribute
{
    private readonly string[] _extensions = extensions;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !_extensions.Contains(extension))
            {
                return new ValidationResult($"Apenas os seguintes formatos são permitidos: {string.Join(", ", _extensions)}");
            }
        }

        return ValidationResult.Success;
    }
}

public class MaxFileSizeAttribute(int maxFileSizeInMB) : ValidationAttribute
{
    private readonly int _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            if (file.Length > _maxFileSizeInBytes)
            {
                return new ValidationResult($"O arquivo deve ter no máximo {_maxFileSizeInBytes / (1024 * 1024)} MB");
            }
        }

        return ValidationResult.Success;
    }
}
