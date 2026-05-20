namespace CarRental.Abstraction;

public abstract class TimeStamp
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}