namespace WebScheduler.Api.Models.Validators;

using System.ComponentModel.DataAnnotations;
using Cronos;

/// <summary>
/// Validates if a given value is a valid cron expression
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CronExpressionAttribute : ValidationAttribute
{
    public CronExpressionAttribute(CronFormat cronFormat) => this.CronFormat = cronFormat;

    public CronFormat CronFormat { get; }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString() ?? string.Empty))
        {
            return new ValidationResult("Cron Expression is required");
        }

        try
        {
            _ = CronExpression.Parse(value?.ToString(), this.CronFormat);
        }
        catch (CronFormatException ex)
        {
            return new ValidationResult(this.ErrorMessage ?? ex.Message);
        }

        return ValidationResult.Success;
    }
}
