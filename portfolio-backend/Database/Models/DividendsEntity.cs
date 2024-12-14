using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models;

[Table("dividenden")]
public class DividendsEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("wertpapier_id")]
    public int WertpapierId { get; set; }

    [ForeignKey("WertpapierId")]
    public virtual StockEntity Wertpapier { get; set; } = null!; // Navigation property to WertpapierEntity

    [Required]
    [Column("depot_id")]
    public int DepotId { get; set; }

    [ForeignKey("DepotId")]
    public virtual DepotEntity Depot { get; set; } = null!; // Navigation property to DepotEntity

    [Required]
    [Column("transaktion")]
    public required char Transaktion { get; set; } = 'D'; // Default value: 'D'

    [Required]
    [Column("extag")]
    public required DateTime ExTag { get; set; }

    [Required]
    [Column("buchung")]
    public required DateTime Buchung { get; set; }

    [Required]
    [Column("stueck")]
    public int Stueck { get; set; }

    [Required]
    [Column("dividende")]
    public float Dividende { get; set; } // Dividende Brutto

    [Required]
    [Column("betrag")]
    public float Betrag { get; set; } // Betrag Netto

    [Required]
    [Column("waehrung")]
    public required string Waehrung { get; set; } // Währung Dividende

    [Required]
    [Column("betrag_stueck")]
    public float BetragStueck { get; set; } // Dividende Stück in Währung

    [Required]
    [Column("devisenkurs")]
    public float Devisenkurs { get; set; } // Devisenkurs EUR/Devisen
}