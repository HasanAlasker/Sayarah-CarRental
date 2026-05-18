namespace CarRental.Models;

public class Type
{
    public int Id { get; set; }
    public string Name { get; set; }    
    
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}