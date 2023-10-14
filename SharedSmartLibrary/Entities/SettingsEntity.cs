using System.ComponentModel.DataAnnotations;


namespace SharedSmartLibrary.Entities;

public class SettingsEntity
{
    [Key]
    public string ConnectionString { get; set; } = null!;
}
