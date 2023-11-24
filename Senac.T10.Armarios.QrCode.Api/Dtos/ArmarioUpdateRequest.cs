namespace Senac.T10.Armarios.QrCode.Api.Dtos
{
    public class ArmarioUpdateRequest
    {
        public int Id { get; set; } = 0;
        public string? Descricao { get; set; }
        public string? QrCodeBase64 { get; set; }
    }
}
