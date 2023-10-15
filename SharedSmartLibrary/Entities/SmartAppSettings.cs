using System.ComponentModel.DataAnnotations;

namespace SharedSmartLibrary.Entities;

public class SmartAppSettings
{
    [Key]
    public string ConnectionString { get; set; } = null!;
}
