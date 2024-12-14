using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models
{
    [Table("transaktionen")]
    public class TransactionEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("wertpapier_id")]
        public int WertpapierId { get; set; }

        [ForeignKey("WertpapierId")]
        public virtual StockEntity Wertpapier { get; set; } = null!;

        [Required]
        [Column("depot_id")]
        public int DepotId { get; set; }

        [ForeignKey("DepotId")]
        public virtual DepotEntity Depot { get; set; } = null!;

        [Required]
        [Column("transaktion")]
        public required char Transaktion { get; set; }

        [Required]
        [Column("datum")]
        public required DateTime Datum { get; set; }

        [Required]
        [Column("buchung")]
        public required DateTime Buchung { get; set; }

        [Required]
        [Column("stueck")]
        public float Stueck { get; set; }

        [Required]
        [Column("kurs")]
        public float Kurs { get; set; }

        [Required]
        [Column("spesen")]
        public float Spesen { get; set; }

        [Required]
        [Column("steuern")]
        public float Steuern { get; set; }

        [Required]
        [Column("abschlag")]
        public float Abschlag { get; set; }

        [Required]
        [Column("bemerkung")]
        public required string Bemerkung { get; set; }
    }
}
