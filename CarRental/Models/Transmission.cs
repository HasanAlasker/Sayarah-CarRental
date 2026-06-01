namespace CarRental.Models;

public class Transmission
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}