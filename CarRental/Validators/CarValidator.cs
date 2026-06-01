using CarRental.ViewModels;
using FluentValidation;

namespace CarRental.Validators;

public class CarValidator : AbstractValidator<CarViewModel>
{
    public CarValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Make).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.Now.Year + 1);
        RuleFor(x => x.Seats).InclusiveBetween(1, 20);
        RuleFor(x => x.CostPerDay).GreaterThan(0);
        RuleFor(x => x.FuelId).GreaterThan(0).WithMessage("Please select a fuel type.");
        RuleFor(x => x.TypeId).GreaterThan(0).WithMessage("Please select a car type.");
        RuleFor(x => x.TransmissionId).GreaterThan(0).WithMessage("Please select a transmission type.");
    }
}
