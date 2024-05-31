using System.Net;
using System.Net.Mail;
using ContactFormApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContactFormApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController: ControllerBase
    {   
        private readonly IConfiguration _configuration;

        public ContactController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody] ContactFromModel form)
        {
                var emailSettings = _configuration.GetSection("EmailSettings");
                if (emailSettings == null)
                {
                    return StatusCode(500, new { error = "Email setting not found."});
                }


                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["Port"] ?? throw new ArgumentException("Port")),
                    Credentials = new NetworkCredential(emailSettings["Username"], Environment.GetEnvironmentVariable("SMTP_PASSWORD")),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["FromEmail"]?? throw new ArgumentException("FromEmail"), emailSettings["FromName"]),
                    Subject = "Thank you for your contact!",
                    Body = $"Hello {form.Name},\n\nThank you for reaching out to us. We have received your message regarding \"{form.Subject}\". Our team will get back to you shortly.\n\nBest regards,\nVeloretti Bikes",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(form.Email);

                try 
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    return Ok(new { message = "Email send Successfully"});
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "Error sending email", details = ex.Message });
                }
        }
    }
}