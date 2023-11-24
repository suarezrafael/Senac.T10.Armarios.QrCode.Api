using System.ComponentModel.DataAnnotations;

namespace Senac.T10.Armarios.QrCode.Api.Dtos
{
    public class ArmarioResponse
    {
        public int Id { get; set; } = 0;

        public string? Descricao { get; set; }

        public string QrCodeBase64 { get; set; }

        public string Url { get; set; }
    }
}
