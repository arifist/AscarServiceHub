using System.Text.Json.Serialization;

namespace AscarServiceHub.Models;

public class ServiceRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string RecordNumber { get; set; } = "";
    public DateTime EntryDate { get; set; } = DateTime.Now;
    public DateTime? ExitDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }

    // Status & Priority
    public ServiceStatus Status { get; set; } = ServiceStatus.Waiting;
    public ServicePriority Priority { get; set; } = ServicePriority.Normal;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    // Customer complaint & technical assessment (separate)
    public string CustomerComplaint { get; set; } = "";   // Müşterinin beyanı
    public string DamageDescription { get; set; } = "";   // Teknik tespit (işçi notu)
    public string? PreInspectionNotes { get; set; }        // Ön kontrol notları (mevcut çizik vb.)

    // Work
    public string? WorkDone { get; set; }
    public string? TechnicianNotes { get; set; }
    public string? AssignedTechnician { get; set; }
    public string? ParkingSpot { get; set; }              // Atölyedeki konum (Kabin 1, Açık Alan vb.)

    // Mileage
    public int? EntryMileage { get; set; }
    public int? ExitMileage { get; set; }

    // Cost
    public decimal EstimatedCost { get; set; }
    public decimal? FinalCost { get; set; }

    // Insurance
    public bool HasInsurance { get; set; }
    public string? InsuranceCompany { get; set; }
    public string? InsurancePolicyNo { get; set; }

    // Damage items
    public List<DamageItem> DamageItems { get; set; } = new();

    // Navigation (JSON ignored)
    [JsonIgnore] public Vehicle? Vehicle { get; set; }
    [JsonIgnore] public List<DamagePhoto> Photos { get; set; } = new();
    [JsonIgnore] public Invoice? Invoice { get; set; }

    // Computed
    public int DaysInWorkshop => (int)(((ExitDate ?? DateTime.Now) - EntryDate).TotalDays);
}

public class DamageItem
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public string? Area { get; set; }
    public bool IsFixed { get; set; }
    public decimal EstimatedCost { get; set; }
}
