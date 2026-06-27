namespace AscarServiceHub.Models;

public enum ServiceStatus
{
    Waiting = 0,       // Bekliyor
    Diagnosing = 1,    // İnceleniyor
    InProgress = 2,    // Onarımda
    WaitingParts = 3,  // Parça Bekleniyor
    Completed = 4,     // Tamamlandı
    Delivered = 5      // Teslim Edildi
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Partial = 2,
    Waived = 3
}

public enum ServicePriority
{
    Low = 0,     // Düşük
    Normal = 1,  // Normal
    High = 2,    // Yüksek
    Urgent = 3   // Acil
}

public enum ApprovalStatus
{
    Pending = 0,   // Onay Bekleniyor
    Approved = 1,  // Onaylandı
    Rejected = 2   // Reddedildi
}
