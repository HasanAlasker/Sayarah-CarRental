namespace CarRental.Models;

public class Rental
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CarId { get; set; }
    public int OwnerId { get; set; }
    public int RenterId { get; set; }
    public decimal TotalPrice { get; set; }
}