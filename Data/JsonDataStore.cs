using System.Text.Json;
using System.Text.Json.Serialization;
using AscarServiceHub.Models;

namespace AscarServiceHub.Data;

public class JsonDataStore
{
    private readonly string _dataDir;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOpts;

    private List<Customer> _customers = new();
    private List<Vehicle> _vehicles = new();
    private List<ServiceRecord> _serviceRecords = new();
    private List<DamagePhoto> _photos = new();
    private List<Invoice> _invoices = new();

    public JsonDataStore(IConfiguration configuration)
    {
        _jsonOpts = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        _dataDir = configuration["Database:JsonDataDir"] ?? "App_Data";
        Directory.CreateDirectory(_dataDir);
        LoadAll();
        SeedIfEmpty();
    }

    // ─── Load / Save ──────────────────────────────────────────────────────────

    private void LoadAll()
    {
        _customers = Load<Customer>("customers.json");
        _vehicles = Load<Vehicle>("vehicles.json");
        _serviceRecords = Load<ServiceRecord>("service_records.json");
        _photos = Load<DamagePhoto>("photos.json");
        _invoices = Load<Invoice>("invoices.json");
    }

    private List<T> Load<T>(string file)
    {
        var path = Path.Combine(_dataDir, file);
        if (!File.Exists(path)) return new();
        try { return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), _jsonOpts) ?? new(); }
        catch { return new(); }
    }

    private void Save<T>(List<T> list, string file)
        => File.WriteAllText(Path.Combine(_dataDir, file), JsonSerializer.Serialize(list, _jsonOpts));

    // ─── Seed ─────────────────────────────────────────────────────────────────

    private void SeedIfEmpty()
    {
        if (_customers.Any()) return;

        _customers = new List<Customer>
        {
            new() { Id=1, FullName="Mehmet Yılmaz",   Phone="0532 111 2233", Email="mehmet@example.com",    Address="Atatürk Cad. No:12, Kadıköy / İstanbul",   CreatedAt=new DateTime(2026,1,10) },
            new() { Id=2, FullName="Ayşe Demir",       Phone="0541 444 5566", Email="ayse.demir@gmail.com",  Address="Cumhuriyet Mah. 45/3, Çankaya / Ankara",    CreatedAt=new DateTime(2026,2,5)  },
            new() { Id=3, FullName="Hasan Kaya",       Phone="0555 777 8899", Email="hasan.kaya@hotmail.com",Address="Bağcılar, İstanbul",                        CreatedAt=new DateTime(2026,2,20) },
            new() { Id=4, FullName="Fatma Çelik",      Phone="0544 333 4455", Email=null,                    Address="Nilüfer, Bursa",                            CreatedAt=new DateTime(2026,3,8)  },
            new() { Id=5, FullName="Ali Öztürk",       Phone="0533 222 3344", Email="ali@firma.com",         Address="Konak, İzmir",          CompanyName="Öztürk Nakliyat",  CreatedAt=new DateTime(2026,3,15) },
            new() { Id=6, FullName="Zeynep Arslan",    Phone="0545 666 7788", Email="zeynep@example.com",    Address="Şişli, İstanbul",                           CreatedAt=new DateTime(2026,4,1)  },
            new() { Id=7, FullName="Kemal Şahin",      Phone="0537 555 9911", Email=null,                    Address="Pendik, İstanbul",                          CreatedAt=new DateTime(2026,5,10) },
        };

        _vehicles = new List<Vehicle>
        {
            new() { Id=1, CustomerId=1, Plate="34 AKL 123", Brand="Toyota",     Model="Corolla",  Year=2018, Color="Beyaz",   FuelType="Benzin", TransmissionType="Otomatik", Vin="JT2BF22K1W0155803" },
            new() { Id=2, CustomerId=1, Plate="06 BCD 456", Brand="Ford",       Model="Focus",    Year=2020, Color="Siyah",   FuelType="Dizel",  TransmissionType="Manuel",   Vin="WF0FXXGCDFJR12345" },
            new() { Id=3, CustomerId=2, Plate="41 EFG 789", Brand="Renault",    Model="Clio",     Year=2019, Color="Kırmızı", FuelType="Benzin", TransmissionType="Manuel",   Vin="VF1BB0B0H12345678" },
            new() { Id=4, CustomerId=3, Plate="34 HIJ 012", Brand="Volkswagen", Model="Passat",   Year=2021, Color="Gümüş",   FuelType="Dizel",  TransmissionType="Otomatik", Vin="WVWZZZ3CZ7E123456" },
            new() { Id=5, CustomerId=4, Plate="16 KLM 345", Brand="Fiat",       Model="Egea",     Year=2022, Color="Lacivert",FuelType="Benzin", TransmissionType="Manuel",   Vin="ZFA31200003123456" },
            new() { Id=6, CustomerId=5, Plate="35 NOP 678", Brand="Honda",      Model="Civic",    Year=2017, Color="Mavi",    FuelType="Benzin", TransmissionType="Otomatik", Vin="JHMES9650GS123456" },
            new() { Id=7, CustomerId=2, Plate="34 RST 901", Brand="Hyundai",    Model="i20",      Year=2020, Color="Gri",     FuelType="Benzin", TransmissionType="Manuel",   Vin="TMAJ3811XLJ123456" },
            new() { Id=8, CustomerId=6, Plate="34 UVW 234", Brand="Dacia",      Model="Logan",    Year=2021, Color="Beyaz",   FuelType="LPG",    TransmissionType="Manuel",   Vin="UU1LSDB4B56123456" },
            new() { Id=9, CustomerId=7, Plate="34 XYZ 567", Brand="Mercedes",   Model="C200",     Year=2023, Color="Siyah",   FuelType="Benzin", TransmissionType="Otomatik", Vin="WDD2050221R123456" },
        };

        var now = DateTime.Now;
        _serviceRecords = new List<ServiceRecord>
        {
            new()
            {
                Id=1, VehicleId=1, RecordNumber="ASC-2026-001",
                EntryDate=now.AddDays(-18), ExitDate=now.AddDays(-14),
                Status=ServiceStatus.Delivered, Priority=ServicePriority.Normal,
                ApprovalStatus=ApprovalStatus.Approved,
                CustomerComplaint="Ön tampon hasar görmüş, sol far çalışmıyor.",
                DamageDescription="Ön tampon kırık ve deforme. Sol far soketi hasar görmüş, far kırık. Sol ön çamurluk üzerinde boyasız göçük.",
                PreInspectionNotes="Sağ aynada ufak çizik mevcut (müşteriye gösterildi).",
                WorkDone="Sol far değiştirildi (orijinal parça). Ön tampon boyasız göçük düzeltme + boya. Sol çamurluk düzleme ve boya.",
                TechnicianNotes="Far soketinde ufak kırık vardı, yenisiyle değiştirildi. Müşteri bilgilendirildi.",
                EntryMileage=87450, ExitMileage=87455,
                EstimatedCost=4500, FinalCost=4200,
                AssignedTechnician="Kadir Usta", ParkingSpot="Kabin 1",
                EstimatedDeliveryDate=now.AddDays(-13),
                DamageItems=new List<DamageItem>
                {
                    new(){Id=1,Description="Sol far değişimi",Area="Ön Sol",IsFixed=true,EstimatedCost=1800},
                    new(){Id=2,Description="Tampon boyasız göçük düzeltme",Area="Ön Merkez",IsFixed=true,EstimatedCost=1200},
                    new(){Id=3,Description="Çamurluk düzleme + boya",Area="Ön Sol",IsFixed=true,EstimatedCost=500},
                }
            },
            new()
            {
                Id=2, VehicleId=3, RecordNumber="ASC-2026-002",
                EntryDate=now.AddDays(-10), ExitDate=now.AddDays(-9),
                Status=ServiceStatus.Delivered, Priority=ServicePriority.Normal,
                ApprovalStatus=ApprovalStatus.Approved,
                CustomerComplaint="Periyodik bakım zamanı. Yağ değişimi ve genel kontrol.",
                DamageDescription="Motor yağı eskimiş. Hava filtresi tıkalı. Ön fren balataları %15 kaldı.",
                WorkDone="Motor yağı ve filtre değişimi (5W30 Full Sentetik). Hava filtresi değişimi. Ön fren balataları değişimi.",
                TechnicianNotes="Arka fren disklerinde hafif pas var, müşteri şimdilik değiştirmek istemedi.",
                EntryMileage=52300, ExitMileage=52305, EstimatedCost=2800, FinalCost=2650,
                AssignedTechnician="Murat Teknisyen", ParkingSpot="Kabin 2",
                EstimatedDeliveryDate=now.AddDays(-8),
            },
            new()
            {
                Id=3, VehicleId=4, RecordNumber="ASC-2026-003",
                EntryDate=now.AddDays(-7),
                Status=ServiceStatus.InProgress, Priority=ServicePriority.High,
                ApprovalStatus=ApprovalStatus.Approved,
                CustomerComplaint="Dolu hasarı var, kaput ve kapılarda göçükler.",
                DamageDescription="Sağ ön kapıda dolu hasarı (12 göçük). Kaput üzerinde yaygın küçük göçükler. Motor kapağı hafif eğilmiş.",
                WorkDone="Sağ ön kapı PDR başlandı. Kaput boyasız göçük düzeltme devam ediyor.",
                TechnicianNotes="Kaput menteşesinde hafif bükülme var.",
                EntryMileage=41200, EstimatedCost=8900,
                HasInsurance=true, InsuranceCompany="Allianz", InsurancePolicyNo="TR-2026-55443",
                AssignedTechnician="Kadir Usta", ParkingSpot="Açık Alan B",
                EstimatedDeliveryDate=now.AddDays(3),
                DamageItems=new List<DamageItem>
                {
                    new(){Id=4,Description="Sağ ön kapı PDR",Area="Sağ Yan",IsFixed=false,EstimatedCost=3500},
                    new(){Id=5,Description="Kaput boyasız göçük düzeltme",Area="Kaput",IsFixed=false,EstimatedCost=2800},
                    new(){Id=6,Description="Motor kapağı düzleme",Area="Motor",IsFixed=false,EstimatedCost=1200},
                }
            },
            new()
            {
                Id=4, VehicleId=5, RecordNumber="ASC-2026-004",
                EntryDate=now.AddDays(-5),
                Status=ServiceStatus.WaitingParts, Priority=ServicePriority.Normal,
                ApprovalStatus=ApprovalStatus.Approved,
                CustomerComplaint="Arka tampon hasar gördü, egzoz ses çıkarıyor.",
                DamageDescription="Arka tampon deforme ve boyasız. Egzoz borusu eğilmiş, ek yeri çatlak. Sol stop lambası çatlak.",
                WorkDone="Arka tampon söküldü. Egzoz parçası sipariş edildi (3-4 gün).",
                EntryMileage=28900, EstimatedCost=3200,
                AssignedTechnician="Murat Teknisyen", ParkingSpot="Kabin 3",
                EstimatedDeliveryDate=now.AddDays(4),
            },
            new()
            {
                Id=5, VehicleId=6, RecordNumber="ASC-2026-005",
                EntryDate=now.AddDays(-3),
                Status=ServiceStatus.Diagnosing, Priority=ServicePriority.Urgent,
                ApprovalStatus=ApprovalStatus.Pending,
                CustomerComplaint="Vites geçişlerinde sertlik var, geri vitese girmekte güçlük çekiyorum.",
                DamageDescription="Şanzıman inceleme devam ediyor. Ön kontrol: vites kolundan gelen titreşim hissediliyor.",
                EntryMileage=134500, EstimatedCost=0,
                AssignedTechnician="Kadir Usta", ParkingSpot="Kabin 2",
                EstimatedDeliveryDate=now.AddDays(5),
            },
            new()
            {
                Id=6, VehicleId=2, RecordNumber="ASC-2026-006",
                EntryDate=now.AddDays(-1),
                Status=ServiceStatus.Waiting, Priority=ServicePriority.Low,
                ApprovalStatus=ApprovalStatus.Pending,
                CustomerComplaint="Klima soğutmuyor. İçeride kötü koku var.",
                DamageDescription="Klima gaz bakımı ve filtre değişimi gerekiyor.",
                EntryMileage=78200, EstimatedCost=1800,
                ParkingSpot="Açık Alan A",
                EstimatedDeliveryDate=now.AddDays(2),
            },
            new()
            {
                Id=7, VehicleId=7, RecordNumber="ASC-2026-007",
                EntryDate=now.AddDays(-4),
                Status=ServiceStatus.InProgress, Priority=ServicePriority.Normal,
                ApprovalStatus=ApprovalStatus.Approved,
                CustomerComplaint="Sol ayna kırılmış, çamurluğa çarpmışlar.",
                DamageDescription="Sol ön çamurluk deforme. Sürücü tarafı kapı aynası tamamen kırık.",
                WorkDone="Kapı aynası değiştirildi (yeni parça). Çamurluk düzleme işlemi devam ediyor.",
                EntryMileage=36700, EstimatedCost=5600,
                AssignedTechnician="Murat Teknisyen", ParkingSpot="Kabin 1",
                EstimatedDeliveryDate=now.AddDays(2),
                DamageItems=new List<DamageItem>
                {
                    new(){Id=7,Description="Sol kapı aynası değişimi",Area="Sol Yan",IsFixed=true,EstimatedCost=1800},
                    new(){Id=8,Description="Sol ön çamurluk düzleme + boya",Area="Sol Yan",IsFixed=false,EstimatedCost=2400},
                }
            },
            new()
            {
                Id=8, VehicleId=9, RecordNumber="ASC-2026-008",
                EntryDate=now,
                Status=ServiceStatus.Waiting, Priority=ServicePriority.Normal,
                ApprovalStatus=ApprovalStatus.Pending,
                CustomerComplaint="Arka tampon sağ köşede çizik ve göçük var. Düzeltme ve boya istiyorum.",
                DamageDescription="Arka sağ köşede 15x8 cm göçük, boya soyulmuş.",
                EntryMileage=12300, EstimatedCost=2800,
                HasInsurance=true, InsuranceCompany="AXA", InsurancePolicyNo="AX-2026-88811",
                ParkingSpot="Açık Alan C",
                EstimatedDeliveryDate=now.AddDays(4),
            },
        };

        _photos = new List<DamagePhoto>
        {
            new(){Id=1,ServiceRecordId=1,FileName="placeholder.png",OriginalFileName="on_tampon.jpg",DamageAreaLabel="Ön Merkez",Description="Ön tampon hasarı",TakenAt=now.AddDays(-18)},
            new(){Id=2,ServiceRecordId=1,FileName="placeholder.png",OriginalFileName="sol_far.jpg",DamageAreaLabel="Ön Sol",Description="Kırık sol far",TakenAt=now.AddDays(-18)},
            new(){Id=3,ServiceRecordId=3,FileName="placeholder.png",OriginalFileName="dolu_kaput.jpg",DamageAreaLabel="Kaput",Description="Dolu hasarı",TakenAt=now.AddDays(-7)},
            new(){Id=4,ServiceRecordId=7,FileName="placeholder.png",OriginalFileName="ayna_kirik.jpg",DamageAreaLabel="Sol Yan",Description="Kırık dikiz aynası",TakenAt=now.AddDays(-4)},
        };

        _invoices = new List<Invoice>
        {
            new()
            {
                Id=1, ServiceRecordId=1, InvoiceNumber="ASC-INV-2026-001",
                IssuedAt=now.AddDays(-14), PaymentStatus=PaymentStatus.Paid, TaxRate=0.20m,
                Items=new List<InvoiceItem>
                {
                    new(){Id=1,Description="Sol Far (Orijinal Parça)",Quantity=1,Unit="Adet",UnitPrice=1800},
                    new(){Id=2,Description="Boyasız Göçük Düzeltme (Ön Tampon)",Quantity=1,Unit="İşçilik",UnitPrice=1200},
                    new(){Id=3,Description="Sol Çamurluk Düzleme + Boya",Quantity=1,Unit="İşçilik",UnitPrice=500},
                }
            },
            new()
            {
                Id=2, ServiceRecordId=2, InvoiceNumber="ASC-INV-2026-002",
                IssuedAt=now.AddDays(-9), PaymentStatus=PaymentStatus.Paid, TaxRate=0.20m,
                Items=new List<InvoiceItem>
                {
                    new(){Id=4,Description="Motor Yağı 5W30 Full Sentetik (4L)",Quantity=1,Unit="Adet",UnitPrice=850},
                    new(){Id=5,Description="Yağ Filtresi",Quantity=1,Unit="Adet",UnitPrice=150},
                    new(){Id=6,Description="Hava Filtresi",Quantity=1,Unit="Adet",UnitPrice=220},
                    new(){Id=7,Description="Ön Fren Balatası Takımı",Quantity=1,Unit="Takım",UnitPrice=680},
                    new(){Id=8,Description="Yağ + Filtre Değişimi İşçilik",Quantity=1,Unit="İşçilik",UnitPrice=250},
                    new(){Id=9,Description="Balata Değişim İşçilik",Quantity=1,Unit="İşçilik",UnitPrice=200},
                }
            },
        };

        SaveAll();
    }

    private void SaveAll()
    {
        Save(_customers, "customers.json");
        Save(_vehicles, "vehicles.json");
        Save(_serviceRecords, "service_records.json");
        Save(_photos, "photos.json");
        Save(_invoices, "invoices.json");
    }

    // ─── Dashboard ────────────────────────────────────────────────────────────

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var now = DateTime.Now;
            var thisMonth = new DateTime(now.Year, now.Month, 1);
            var completed = _serviceRecords.Where(r => r.Status == ServiceStatus.Delivered && r.ExitDate >= thisMonth).ToList();

            return new DashboardStats
            {
                TotalCustomers = _customers.Count,
                TotalVehicles = _vehicles.Count,
                TotalRecords = _serviceRecords.Count,
                ActiveRecords = _serviceRecords.Count(r => r.Status != ServiceStatus.Delivered),
                WaitingCount = _serviceRecords.Count(r => r.Status == ServiceStatus.Waiting),
                DiagnosingCount = _serviceRecords.Count(r => r.Status == ServiceStatus.Diagnosing),
                InProgressCount = _serviceRecords.Count(r => r.Status == ServiceStatus.InProgress),
                WaitingPartsCount = _serviceRecords.Count(r => r.Status == ServiceStatus.WaitingParts),
                CompletedCount = _serviceRecords.Count(r => r.Status == ServiceStatus.Completed),
                DeliveredCount = _serviceRecords.Count(r => r.Status == ServiceStatus.Delivered),
                CompletedThisMonth = completed.Count,
                RevenueThisMonth = completed.Sum(r => r.FinalCost ?? 0),
                TotalRevenue = _serviceRecords.Where(r => r.FinalCost.HasValue).Sum(r => r.FinalCost!.Value),
                UrgentCount = _serviceRecords.Count(r => r.Priority == ServicePriority.Urgent && r.Status != ServiceStatus.Delivered),
            };
        }
        finally { _lock.Release(); }
    }

    public async Task<List<ServiceRecord>> GetActiveServiceRecordsAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var vehicleMap = _vehicles.ToDictionary(v => v.Id);
            var customerMap = _customers.ToDictionary(c => c.Id);
            var records = _serviceRecords
                .Where(r => r.Status != ServiceStatus.Delivered)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.EntryDate)
                .ToList();
            foreach (var r in records)
            {
                r.Vehicle = vehicleMap.GetValueOrDefault(r.VehicleId);
                if (r.Vehicle != null) r.Vehicle.Customer = customerMap.GetValueOrDefault(r.Vehicle.CustomerId);
                r.Photos = _photos.Where(p => p.ServiceRecordId == r.Id).ToList();
            }
            return records;
        }
        finally { _lock.Release(); }
    }

    // ─── Customer ─────────────────────────────────────────────────────────────

    public async Task<List<Customer>> GetCustomersAsync()
    {
        await _lock.WaitAsync();
        try { return _customers.OrderByDescending(c => c.CreatedAt).ToList(); }
        finally { _lock.Release(); }
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        await _lock.WaitAsync();
        try { return _customers.FirstOrDefault(c => c.Id == id); }
        finally { _lock.Release(); }
    }

    public async Task<Customer?> FindCustomerByPhoneAsync(string phone)
    {
        await _lock.WaitAsync();
        try
        {
            var clean = phone.Replace(" ", "").Replace("-", "");
            return _customers.FirstOrDefault(c =>
                c.Phone.Replace(" ", "").Replace("-", "").Contains(clean, StringComparison.OrdinalIgnoreCase));
        }
        finally { _lock.Release(); }
    }

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        await _lock.WaitAsync();
        try
        {
            customer.Id = _customers.Count == 0 ? 1 : _customers.Max(c => c.Id) + 1;
            customer.CreatedAt = DateTime.Now;
            _customers.Add(customer);
            Save(_customers, "customers.json");
            return customer;
        }
        finally { _lock.Release(); }
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        await _lock.WaitAsync();
        try
        {
            var idx = _customers.FindIndex(c => c.Id == customer.Id);
            if (idx >= 0) { _customers[idx] = customer; Save(_customers, "customers.json"); }
        }
        finally { _lock.Release(); }
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await _lock.WaitAsync();
        try { _customers.RemoveAll(c => c.Id == id); Save(_customers, "customers.json"); }
        finally { _lock.Release(); }
    }

    // ─── Vehicle ──────────────────────────────────────────────────────────────

    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var customerMap = _customers.ToDictionary(c => c.Id);
            var vehicles = _vehicles.OrderByDescending(v => v.CreatedAt).ToList();
            foreach (var v in vehicles) v.Customer = customerMap.GetValueOrDefault(v.CustomerId);
            return vehicles;
        }
        finally { _lock.Release(); }
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var v = _vehicles.FirstOrDefault(v => v.Id == id);
            if (v != null) v.Customer = _customers.FirstOrDefault(c => c.Id == v.CustomerId);
            return v;
        }
        finally { _lock.Release(); }
    }

    public async Task<Vehicle?> FindVehicleByPlateAsync(string plate)
    {
        await _lock.WaitAsync();
        try
        {
            var clean = plate.Replace(" ", "").ToUpperInvariant();
            var v = _vehicles.FirstOrDefault(v => v.Plate.Replace(" ", "").ToUpperInvariant() == clean);
            if (v != null) v.Customer = _customers.FirstOrDefault(c => c.Id == v.CustomerId);
            return v;
        }
        finally { _lock.Release(); }
    }

    public async Task<List<Vehicle>> GetVehiclesByCustomerIdAsync(int customerId)
    {
        await _lock.WaitAsync();
        try { return _vehicles.Where(v => v.CustomerId == customerId).ToList(); }
        finally { _lock.Release(); }
    }

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        await _lock.WaitAsync();
        try
        {
            vehicle.Id = _vehicles.Count == 0 ? 1 : _vehicles.Max(v => v.Id) + 1;
            vehicle.CreatedAt = DateTime.Now;
            _vehicles.Add(vehicle);
            Save(_vehicles, "vehicles.json");
            return vehicle;
        }
        finally { _lock.Release(); }
    }

    public async Task UpdateVehicleAsync(Vehicle vehicle)
    {
        await _lock.WaitAsync();
        try
        {
            var idx = _vehicles.FindIndex(v => v.Id == vehicle.Id);
            if (idx >= 0) { _vehicles[idx] = vehicle; Save(_vehicles, "vehicles.json"); }
        }
        finally { _lock.Release(); }
    }

    // ─── ServiceRecord ────────────────────────────────────────────────────────

    public async Task<List<ServiceRecord>> GetServiceRecordsAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var vehicleMap = _vehicles.ToDictionary(v => v.Id);
            var customerMap = _customers.ToDictionary(c => c.Id);
            var records = _serviceRecords.OrderByDescending(r => r.EntryDate).ToList();
            foreach (var r in records)
            {
                r.Vehicle = vehicleMap.GetValueOrDefault(r.VehicleId);
                if (r.Vehicle != null) r.Vehicle.Customer = customerMap.GetValueOrDefault(r.Vehicle.CustomerId);
                r.Photos = _photos.Where(p => p.ServiceRecordId == r.Id).ToList();
            }
            return records;
        }
        finally { _lock.Release(); }
    }

    public async Task<ServiceRecord?> GetServiceRecordByIdAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var r = _serviceRecords.FirstOrDefault(r => r.Id == id);
            if (r == null) return null;
            r.Vehicle = _vehicles.FirstOrDefault(v => v.Id == r.VehicleId);
            if (r.Vehicle != null) r.Vehicle.Customer = _customers.FirstOrDefault(c => c.Id == r.Vehicle.CustomerId);
            r.Photos = _photos.Where(p => p.ServiceRecordId == r.Id).ToList();
            r.Invoice = _invoices.FirstOrDefault(i => i.ServiceRecordId == r.Id);
            return r;
        }
        finally { _lock.Release(); }
    }

    public async Task<List<ServiceRecord>> GetServiceRecordsByVehicleIdAsync(int vehicleId)
    {
        await _lock.WaitAsync();
        try
        {
            var vMap = _vehicles.ToDictionary(v => v.Id);
            var cMap = _customers.ToDictionary(c => c.Id);
            var records = _serviceRecords.Where(r => r.VehicleId == vehicleId).OrderByDescending(r => r.EntryDate).ToList();
            foreach (var r in records)
            {
                r.Vehicle = vMap.GetValueOrDefault(r.VehicleId);
                if (r.Vehicle != null) r.Vehicle.Customer = cMap.GetValueOrDefault(r.Vehicle.CustomerId);
            }
            return records;
        }
        finally { _lock.Release(); }
    }

    public async Task<ServiceRecord> AddServiceRecordAsync(ServiceRecord record)
    {
        await _lock.WaitAsync();
        try
        {
            record.Id = _serviceRecords.Count == 0 ? 1 : _serviceRecords.Max(r => r.Id) + 1;
            record.RecordNumber = GenerateRecordNumber();
            record.EntryDate = DateTime.Now;
            _serviceRecords.Add(record);
            Save(_serviceRecords, "service_records.json");
            return record;
        }
        finally { _lock.Release(); }
    }

    public async Task UpdateServiceRecordAsync(ServiceRecord record)
    {
        await _lock.WaitAsync();
        try
        {
            var idx = _serviceRecords.FindIndex(r => r.Id == record.Id);
            if (idx >= 0) { _serviceRecords[idx] = record; Save(_serviceRecords, "service_records.json"); }
        }
        finally { _lock.Release(); }
    }

    private string GenerateRecordNumber()
    {
        var year = DateTime.Now.Year;
        var seq = _serviceRecords.Count(r => r.RecordNumber.Contains(year.ToString())) + 1;
        return $"ASC-{year}-{seq:D3}";
    }

    // ─── Photos ───────────────────────────────────────────────────────────────

    public async Task<List<DamagePhoto>> GetPhotosByServiceRecordIdAsync(int serviceRecordId)
    {
        await _lock.WaitAsync();
        try { return _photos.Where(p => p.ServiceRecordId == serviceRecordId).ToList(); }
        finally { _lock.Release(); }
    }

    public async Task<DamagePhoto> AddPhotoAsync(DamagePhoto photo)
    {
        await _lock.WaitAsync();
        try
        {
            photo.Id = _photos.Count == 0 ? 1 : _photos.Max(p => p.Id) + 1;
            photo.TakenAt = DateTime.Now;
            _photos.Add(photo);
            Save(_photos, "photos.json");
            return photo;
        }
        finally { _lock.Release(); }
    }

    public async Task DeletePhotoAsync(int id)
    {
        await _lock.WaitAsync();
        try { _photos.RemoveAll(p => p.Id == id); Save(_photos, "photos.json"); }
        finally { _lock.Release(); }
    }

    // ─── Invoice ──────────────────────────────────────────────────────────────

    public async Task<Invoice?> GetInvoiceByServiceRecordIdAsync(int id)
    {
        await _lock.WaitAsync();
        try { return _invoices.FirstOrDefault(i => i.ServiceRecordId == id); }
        finally { _lock.Release(); }
    }

    public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
    {
        await _lock.WaitAsync();
        try
        {
            invoice.Id = _invoices.Count == 0 ? 1 : _invoices.Max(i => i.Id) + 1;
            invoice.InvoiceNumber = $"ASC-INV-{DateTime.Now.Year}-{_invoices.Count + 1:D3}";
            invoice.IssuedAt = DateTime.Now;
            _invoices.Add(invoice);
            Save(_invoices, "invoices.json");
            return invoice;
        }
        finally { _lock.Release(); }
    }

    public async Task UpdateInvoiceAsync(Invoice invoice)
    {
        await _lock.WaitAsync();
        try
        {
            var idx = _invoices.FindIndex(i => i.Id == invoice.Id);
            if (idx >= 0) { _invoices[idx] = invoice; Save(_invoices, "invoices.json"); }
        }
        finally { _lock.Release(); }
    }
}

public class DashboardStats
{
    public int TotalCustomers { get; set; }
    public int TotalVehicles { get; set; }
    public int TotalRecords { get; set; }
    public int ActiveRecords { get; set; }
    public int WaitingCount { get; set; }
    public int DiagnosingCount { get; set; }
    public int InProgressCount { get; set; }
    public int WaitingPartsCount { get; set; }
    public int CompletedCount { get; set; }
    public int DeliveredCount { get; set; }
    public int CompletedThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UrgentCount { get; set; }
}
