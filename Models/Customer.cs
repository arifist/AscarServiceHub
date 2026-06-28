using System.Text.Json.Serialization;

namespace AscarServiceHub.Models;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }    // TC Kimlik No
    public string? TaxNumber { get; set; }
    public string? CompanyName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public List<Vehicle> Vehicles { get; set; } = new();
}
