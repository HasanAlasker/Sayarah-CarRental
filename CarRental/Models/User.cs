using System.ComponentModel.DataAnnotations;
using CarRental.Abstraction;
using CarRental.enums;

namespace CarRental.Models;

public class User : TimeStamp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public bool AgreedToTermsAndConditions { get; set; }
    
    public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    public ICollection<Car> Cars { get; set; } = new List<Car>(); 
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}