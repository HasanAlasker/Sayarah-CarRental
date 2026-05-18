namespace CarRental.Models;

public class Car
{
    public  int Id { get; set; }
    public string Make { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public decimal PricePerDay { get; set; }
    public int Seats { get; set; }
    public int FuelId { get; set; }
    public int TransmissionId { get; set; }
    public int TypeId { get; set; }
    public int OwnerId { get; set; }
}