namespace CarRental.ViewModels;

public class CarViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Make { get; set; }
    public int Year { get; set; }
    public int Seats { get; set; }
    public decimal CostPerDay { get; set; }
    public int FuelId { get; set; }
    public int TypeId { get; set; }
    public int TransmissionId { get; set; }
}
