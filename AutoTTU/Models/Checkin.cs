using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoTTU.Models
{
    public class Checkin
    {
        [Key]
        public int IdCheckin { get; set; }

        [ForeignKey("Motos")]
        public int IdMoto { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(1)]
        public string AtivoChar { get; set; }  // Armazena "S" ou "N" no banco

        [NotMapped]
        public bool Violada
        {
            get => AtivoChar == "S";
            set => AtivoChar = value ? "S" : "N";
        }

        [Required]
        public string Observacao { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        [MaxLength(2048)]
        public string ImagensUrl { get; set; }


    }
}
