using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Validation;

public static class PaymentValidator
{
    public static void ValidatePaymentCreation(decimal amount, Enrollment enrollment, decimal remainingBalance)
    {
        var errors = new List<string>();

        if (amount <= 0)
        {
            errors.Add("Payment amount must be strictly greater than 0.");
        }

        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            errors.Add("Cannot process payments for a cancelled enrollment.");
        }

        if (amount > remainingBalance)
        {
            errors.Add($"Payment amount of {amount:N2} EGP exceeds remaining outstanding balance of {remainingBalance:N2} EGP.");
        }

        if (errors.Any())
        {
            throw new ValidationException("Payment validation failed.", errors);
        }
    }
}
