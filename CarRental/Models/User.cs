using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MinLength(2)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    public ICollection<Car> Cars { get; set; } = new List<Car>(); 
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}