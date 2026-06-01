using CarRental.ViewModels;
using FluentValidation;

namespace CarRental.Validators;

public class RegisterValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
        RuleFor(x => x.DateOfBirth).NotEmpty().Must(BeAtLeast18).WithMessage("You must be at least 18 years old.");
        RuleFor(x => x.AgreedToTermsAndConditions).Equal(true).WithMessage("You must agree to the terms and conditions.");
    }

    private bool BeAtLeast18(DateTime dob)
    {
        return dob <= DateTime.Today.AddYears(-18);
    }
}
