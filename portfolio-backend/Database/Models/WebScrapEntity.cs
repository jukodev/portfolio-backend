using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models
{
    [Table("webscraper")]
    public class WebScrapEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("wertpapier_id")]
        public required int WertpapierId { get; set; }

        [ForeignKey("WertpapierId")]
        public virtual StockEntity Stock { get; set; } = null!;

        [Required]
        [Column("website")]
        public required string Website { get; set; }

        [Required]
        [Column("market")]
        public required string Market { get; set; }

        [Required]
        [Column("url")]
        public required string Url { get; set; }

        [Required]
        [Column("url_dividenden")]
        public required string UrlDividenden { get; set; }
    }
}
