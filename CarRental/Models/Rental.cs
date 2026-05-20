using CarRental.Abstraction;

namespace CarRental.Models;

public class Rental : TimeStamp
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    
    public int CarId { get; set; }
    public int OwnerId { get; set; }
    public int RenterId { get; set; }
    
    public User Renter { get; set; }
    public Car Car { get; set; }
    public User CarOwner { get; set; }
}