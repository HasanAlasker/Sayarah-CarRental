namespace CarRental.Models;

public class Car
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Make { get; set; }
    public int Year { get; set; }
    public int Seats { get; set; }
    public decimal CostPerDay { get; set; }
    
    public int OwnerId { get; set; }
    public int FuelId { get; set; }
    public int TypeId { get; set; }
    public int TransmissionId { get; set; }
    
    public User Owner { get; set; }
    public Fuel Fuel { get; set; }
    public Type Type { get; set; }
    public Transmission Transmission { get; set; }
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}