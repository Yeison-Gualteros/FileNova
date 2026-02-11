using Service.Contracts;
using Microsoft.Extensions.Options;
using Shared;
using System.Net;
using System.Net.Mail;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendPasswordAsync(string toEmail, string username, string password)
        {
            var bodyHtml = $@"
    <html>
    <body style='font-family: Arial, Helvetica, sans-serif; background-color: #f4f6f8; padding: 20px;'>
        <table width='100%' cellpadding='0' cellspacing='0'>
            <tr>
                <td align='center'>
                    <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 6px; padding: 20px;'>
                        <tr>
                            <td style='text-align: center; padding-bottom: 20px;'>
                                <h2 style='color: #2c3e50; margin: 0;'>Acceso a FileNova</h2>
                            </td>
                        </tr>
                        <tr>
                            <td style='color: #333333; font-size: 14px;'>
                                <p>Estimado/a usuario/a,</p>

                                <p>
                                    Se le ha creado una cuenta de acceso a la plataforma <strong>FileNova</strong>.
                                    A continuación, encontrará sus credenciales:
                                </p>

                                <table width='100%' style='margin: 20px 0;'>
                                    <tr>
                                        <td style='padding: 8px; background-color: #f0f0f0; width: 30%;'><strong>Usuario:</strong></td>
                                        <td style='padding: 8px;'>{username}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px; background-color: #f0f0f0;'><strong>Contraseña:</strong></td>
                                        <td style='padding: 8px;'>{password}</td>
                                    </tr>
                                </table>

                                <p>
                                    Por motivos de seguridad, le recomendamos cambiar su contraseña en su primer inicio de sesión.
                                </p>

                                <p>
                                    Si usted no solicitó este acceso o tiene alguna consulta, por favor comuníquese con el administrador del sistema.
                                </p>

                                <p style='margin-top: 30px;'>
                                    Atentamente,<br />
                                    <strong>{_settings.SenderName}</strong>
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td style='font-size: 12px; color: #777777; padding-top: 20px; border-top: 1px solid #dddddd;'>
                                Este correo es confidencial y está dirigido únicamente al destinatario indicado.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
    </html>";

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = "Credenciales de acceso a FileNova",
                Body = bodyHtml,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Username,
                    _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            await smtp.SendMailAsync(mail);
        }

    }

}
