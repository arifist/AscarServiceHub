using System.Text.Json.Serialization;

namespace AscarServiceHub.Models;

public class Invoice
{
    public int Id { get; set; }
    public int ServiceRecordId { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime IssuedAt { get; set; } = DateTime.Now;
    public List<InvoiceItem> Items { get; set; } = new();
    public decimal TaxRate { get; set; } = 0.20m;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? Notes { get; set; }

    public decimal SubTotal => Items.Sum(i => i.Total);
    public decimal TaxAmount => SubTotal * TaxRate;
    public decimal Total => SubTotal + TaxAmount;

    [JsonIgnore]
    public ServiceRecord? ServiceRecord { get; set; }
}

public class InvoiceItem
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = "Adet";
    public decimal UnitPrice { get; set; }
    public decimal Total => Math.Round(Quantity * UnitPrice, 2);
}
