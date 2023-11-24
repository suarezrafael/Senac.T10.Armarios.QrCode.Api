using System.ComponentModel.DataAnnotations;

namespace Senac.T10.Armarios.QrCode.Api.Models
{
    public class Armario
    {
        [Key]
        public int Id { get; set; } = 0;


        [StringLength(900)]
        public string? Descricao { get; set; }


        [Required]
        public string QrCodeBase64 { get; set; }


        [StringLength(256)]
        public string Url { get; set; }
    }
}
