using System.Net;
using System.Net.Mail;
using System.Text;

namespace Senac.T10.Armarios.QrCode.Api.Helper
{
    public class Email : IEmail
    {
        private readonly IConfiguration _configuration;
        public Email(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Enviar(string email, string assunto, string mensagem)
        {
            string host = _configuration.GetValue<string>("SMTP:Host");
            string nome = _configuration.GetValue<string>("SMTP:Nome");
            string emailorigem = _configuration.GetValue<string>("SMTP:EmailOrigem");
            string senha = _configuration.GetValue<string>("SMTP:Senha");
            int porta = _configuration.GetValue<int>("SMTP:Porta");

            try
            {
                using (var mensagemEmail = new MailMessage())
                {

                    mensagemEmail.From = new MailAddress(emailorigem);
                    mensagemEmail.To.Add(new MailAddress(email, nome));

                    mensagemEmail.Subject = assunto;
                    mensagemEmail.Body = mensagem;
                    mensagemEmail.BodyEncoding = Encoding.UTF8;
                    mensagemEmail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");
                    mensagemEmail.Priority = MailPriority.Normal;

                    using (var smtpCliente = new SmtpClient())
                    {
                        smtpCliente.Host = host;
                        smtpCliente.Port = porta;
                        smtpCliente.EnableSsl = true;
                        smtpCliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpCliente.Credentials = new NetworkCredential(emailorigem, senha);
                        smtpCliente.UseDefaultCredentials = false;

                        smtpCliente.Send(mensagemEmail);
                        return true;
                    }
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                Console.WriteLine("Mensagem : {0} " + ex.Message);
                return false;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Mensagem SMPT Fail : {0} " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mensagem Exception : {0} " + ex.Message);
                return false;
            }
        }
    }
}
