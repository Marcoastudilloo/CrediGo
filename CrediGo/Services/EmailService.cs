using CrediGo.Models;
using System.Net;
using System.Net.Mail;

namespace CrediGo.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string destino, string asunto, string mensaje)
        {
            var remitente = _config["EmailSettings:Remitente"];
            var smtp = new SmtpClient(_config["EmailSettings:Smtp"])
            {
                Port = int.Parse(_config["EmailSettings:Puerto"]),
                Credentials = new NetworkCredential(remitente, _config["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var correo = new MailMessage(remitente, destino, asunto, mensaje);
            await smtp.SendMailAsync(correo);
        }
    }


}
