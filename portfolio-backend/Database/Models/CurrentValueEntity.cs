using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models;

[Table("depot_wert_aktuell")]
public class CurrentValueEntity
{
    [Key]
    [Column("depot_id")]
    public int DepotId { get; set; }

    [ForeignKey("DepotId")]
    public virtual DepotEntity Depot { get; set; } = null!; // Navigation property to DepotEntity

    [Required]
    [Column("pricebasis")]
    public required string PriceBasis { get; set; }

    [Required]
    [Column("date_time")]
    public required DateTime DateTime { get; set; }

    [Required]
    [Column("investment")]
    public float Investment { get; set; }

    [Required]
    [Column("value")]
    public float Value { get; set; }

    [Required]
    [Column("value_net")]
    public float ValueNet { get; set; }

    [Required]
    [Column("profit")]
    public float Profit { get; set; }

    [Required]
    [Column("profit_net")]
    public float ProfitNet { get; set; }

    [Required]
    [Column("tax")]
    public float Tax { get; set; }
}