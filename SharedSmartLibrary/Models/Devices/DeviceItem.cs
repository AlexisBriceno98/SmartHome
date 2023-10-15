namespace SharedSmartLibrary.Models.Devices;

public class DeviceItem
{
    public string DeviceId { get; set; } = null!;
    public string DeviceType { get; set; } = null!;
    public string Vendor { get; set; } = null!;
    public string Location { get; set; } = null!;
    public bool IsActive { get; set; }
}
