using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models
{
    [Table("wertpapiere")]
    public class StockEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Column("first_buy")]
        public DateTime? FirstBuy { get; set; } // Nullable column

        [Column("close_sell")]
        public DateTime? CloseSell { get; set; } // Nullable column

        [Column("close_div")]
        public DateTime? CloseDiv { get; set; } // Nullable column

        [Required]
        [Column("wkn")]
        public required string Wkn { get; set; }

        [Required]
        [Column("symbol")]
        public required string Symbol { get; set; }

        [Required]
        [Column("isin")]
        public required string Isin { get; set; }

        [Required]
        [Column("typ")]
        public required string Typ { get; set; } = "Aktie"; // Default value

        [Required]
        [Column("dividenden_pro_jahr")]
        public int DividendenProJahr { get; set; }

        [Required]
        [Column("dividenden_termine")]
        public required string DividendenTermine { get; set; }

        [Required]
        [Column("dividende_letzte_brutto_eur")]
        public float DividendeLetzteBruttoEur { get; set; }

        [Required]
        [Column("dividenden_steuer_prozent")]
        public int DividendenSteuerProzent { get; set; }

        [Required]
        [Column("goldgewicht")]
        public float Goldgewicht { get; set; }

        public virtual ICollection<WebScrapEntity> WebScraps { get; set; } = [];
    }
}
