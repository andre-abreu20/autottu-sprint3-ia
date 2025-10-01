using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTTU.Models
{
    public class Telemetria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 8)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(11, 8)")]
        public decimal Longitude { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // "online", "offline", "manutencao", "alugada"

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Temperatura { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Velocidade { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Quilometragem { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? NivelCombustivel { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal? RotacaoMotor { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }

        // Navegação
        [ForeignKey("DeviceId")]
        public virtual Motos? Moto { get; set; }
    }
}
