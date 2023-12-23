namespace Senac.T10.Armarios.QrCode.Api.Helper
{
    public interface IEmail
    {
        bool Enviar(string email, string assunto, string mensagem);

    }
}
