using System.Text.Json.Serialization;

namespace AscarServiceHub.Models;

public class DamagePhoto
{
    public int Id { get; set; }
    public int ServiceRecordId { get; set; }
    public string FileName { get; set; } = "";
    public string? OriginalFileName { get; set; }
    public string? DamageAreaLabel { get; set; }
    public string? Description { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public ServiceRecord? ServiceRecord { get; set; }

    public string WebPath => $"/uploads/{FileName}";
}
