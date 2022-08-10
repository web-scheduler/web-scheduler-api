namespace WebScheduler.Client.Http.Models.Validators;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Makes a property required if the specified Property matches the expected value.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class RequiredIfAttribute : ValidationAttribute
{
    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="propertyName">the property name</param>
    /// <param name="expectedValue">the expected value</param>
    public RequiredIfAttribute(string propertyName, object expectedValue)
    {
        this.PropertyName = propertyName;
        this.ExpectedValue = expectedValue;
        this.innerAttribute = new RequiredAttribute();
    }

    /// <summary>
    /// The property name to be required.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// The expected value.
    /// </summary>
    public object ExpectedValue { get; }

    private readonly RequiredAttribute innerAttribute;

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return new ValidationResult("Value can't be null.");
        }
        var dependentValue = validationContext.ObjectInstance.GetType().GetProperty(this.PropertyName)?.GetValue(validationContext.ObjectInstance, null)!;

        if (dependentValue.Equals(this.ExpectedValue) && !this.innerAttribute.IsValid(value))
        {
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), new List<string>() { { validationContext.MemberName! } });
        }

        return ValidationResult.Success;
    }
}
