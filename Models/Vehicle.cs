using System.Text.Json.Serialization;

namespace AscarServiceHub.Models;

public class Vehicle
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Plate { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public string Color { get; set; } = "";
    public string? Vin { get; set; }
    public string? FuelType { get; set; }
    public string? TransmissionType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public Customer? Customer { get; set; }

    [JsonIgnore]
    public List<ServiceRecord> ServiceRecords { get; set; } = new();

    public string DisplayName => $"{Brand} {Model} ({Year})";
}
