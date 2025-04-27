namespace CleanArchitecture.Domain.Entities;

public class Log
{
    public Guid Id { get; set; }
    public string? LogLevel { get; set; }
    public string? LogMessage { get; set; }
    public string? Exception { get; set; }
    public DateTime Logged { get; set; }
}
