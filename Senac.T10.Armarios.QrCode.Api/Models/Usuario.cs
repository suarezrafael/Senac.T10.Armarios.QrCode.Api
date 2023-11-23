using System.ComponentModel.DataAnnotations;

namespace Senac.T10.Armarios.QrCode.Api.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; } = 0;


        [StringLength(200)]
        public string Nome { get; set; }


        [Required]
        [StringLength(150)]
        public string NomeUsuario { get; set; }



        [StringLength(200)]
        public string Senha { get; set; }
    }
}
