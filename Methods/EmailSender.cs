using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using System.Text;

public static class EmailSender
{
    public static Message CreateEmail(string to, string subject, string bodyText)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Velloreti Bikes - Max", "mxxfaria@gmail.com"));
        mailMessage.To.Add(new MailboxAddress("", to));
        mailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = bodyText,
            TextBody = bodyText
        };

        mailMessage.Body = bodyBuilder.ToMessageBody();

        using (var memoryStream = new MemoryStream())
        {
            mailMessage.WriteTo(memoryStream);
            return new Message
            {
                Raw = Base64UrlEncode(memoryStream.ToArray())
            };
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }

    public static async Task SendEmailAsync(GmailService service, Message email)
    {
        await service.Users.Messages.Send(email, "me").ExecuteAsync();
    }
}
