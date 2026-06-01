namespace CarRental.Models;

public class Fuel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}