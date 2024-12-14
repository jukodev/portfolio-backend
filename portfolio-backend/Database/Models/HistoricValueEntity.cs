using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models;

[Table("depot_wert_historisch")]
public class HistoricValueEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("depot_id")]
    public int DepotId { get; set; }

    [ForeignKey("DepotId")]
    public virtual DepotEntity Depot { get; set; } = null!; // Navigation property to DepotEntity

    [Required]
    [Column("date_time")]
    public required DateTime DateTime { get; set; }

    [Required]
    [Column("investment")]
    public float Investment { get; set; }

    [Required]
    [Column("last_value")]
    public float LastValue { get; set; }

    [Required]
    [Column("last_value_net")]
    public float LastValueNet { get; set; }

    [Required]
    [Column("last_profit")]
    public float LastProfit { get; set; }

    [Required]
    [Column("last_profit_net")]
    public float LastProfitNet { get; set; }

    [Required]
    [Column("last_tax")]
    public float LastTax { get; set; }

    [Required]
    [Column("bid_value")]
    public float BidValue { get; set; }

    [Required]
    [Column("bid_value_net")]
    public float BidValueNet { get; set; }

    [Required]
    [Column("bid_profit")]
    public float BidProfit { get; set; }

    [Required]
    [Column("bid_profit_net")]
    public float BidProfitNet { get; set; }

    [Required]
    [Column("bid_tax")]
    public float BidTax { get; set; }
}