using CarRental.enums;

namespace CarRental.ViewModels;

public class RegisterViewModel
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public bool AgreedToTermsAndConditions { get; set; }
}
