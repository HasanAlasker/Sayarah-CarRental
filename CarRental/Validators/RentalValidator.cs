using CarRental.ViewModels;
using FluentValidation;

namespace CarRental.Validators;

public class RentalValidator : AbstractValidator<RentalViewModel>
{
    public RentalValidator()
    {
        RuleFor(x => x.CarId).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty().GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past.");
        RuleFor(x => x.EndDate).NotEmpty().GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}
