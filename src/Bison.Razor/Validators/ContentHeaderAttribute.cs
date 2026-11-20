using System.ComponentModel.DataAnnotations;

namespace Bison.Razor.Validators;

// inspirated by https://stackoverflow.com/a/37638328

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ContentHeaderAttribute(string contentTypes) : ValidationAttribute
{
    private List<string> AllowedContentTypes { get; set; } = [.. contentTypes.Split([','], StringSplitOptions.RemoveEmptyEntries)];

    public override bool IsValid(object? value)
    {
        return value is not IFormFile file || AllowedContentTypes.Any(ct => ct == file.ContentType);
    }
}
