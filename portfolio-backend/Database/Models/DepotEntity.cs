using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio_backend.Database.Models
{
    [Table("depot")]
    public class DepotEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("bezeichnung")]
        public required string Bezeichnung { get; set; }

        [Required]
        [Column("nummer")]
        public required string Nummer { get; set; }

        [Required]
        [Column("besitzer")]
        public required string Besitzer { get; set; }

        [Required]
        [Column("zabbix_id")]
        public required string ZabbixId { get; set; }

    }
}
