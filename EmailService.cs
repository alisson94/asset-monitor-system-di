using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly string host;
    private readonly int port;
    private readonly string username;
    private readonly string password;

    public EmailService(SmtpSettings smtpSettings)
    {
        host = smtpSettings.Host;
        port = smtpSettings.Port;
        username = smtpSettings.Username;
        password = smtpSettings.Password;
    }

    public void SendEmail(string destEmail, string subject, string body)
    {
        try
        {
            SmtpClient client = new SmtpClient(host, port);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(username, password);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(username);
            mail.To.Add(destEmail);
            mail.Subject = subject;
            mail.Body = body;

            client.Send(mail);
            Console.WriteLine("E-mail enviado com sucesso!");
        }catch(Exception e)
        {
            Console.WriteLine($"Falha ao enviar e-mail: {e.Message}");
        }
    }
}