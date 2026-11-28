using System.ComponentModel.DataAnnotations;

namespace Bison.Razor.Validators;

// inspirated by https://stackoverflow.com/a/37638328

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class FileSizeAttribute(long maxFileSize) : ValidationAttribute
{
    private readonly long _maxFileSize = maxFileSize;

    public override bool IsValid(object? value)
    {
        return value is not IFormFile file || file.Length <= _maxFileSize;
    }
}
