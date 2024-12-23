using System.Threading.Tasks;
using System.Net.Mail;
using System.Data;
using System.Drawing;
using System.Net;
using AlertSending.Classes;
using Serilog;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public class EmailSender
{
    string excep = string.Empty;
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public async Task SendEmailAsync(string AlertAddress, string MailID, string Subject, string Initials, string Regards, string Bankname, string Attachment, string dsbody, string bodyattachment, string mailmsgDesc)
    //{
    //    string senderName = CommonConfigurations.SenderMailID;
    //    string UserNM = CommonConfigurations.SenderMailID;
    //    string mailServer = CommonConfigurations.ServerIPAdress;
    //    string senderEmailId = CommonConfigurations.SenderMailID;
    //    string password = CommonConfigurations.SenderMailPassword;
    //    string emaillistTo = string.Empty;
    //    string _emaillistcc = string.Empty;
    //    try
    //    {
    //        MailMessage mail = new MailMessage();
    //        SmtpClient SmtpServer = new SmtpClient(mailServer);
    //        var message = new System.Net.Mail.MailMessage();
    //        message.Body = Initials.Replace("@n", Environment.NewLine);
    //        mail.From = new MailAddress(senderEmailId);
    //        mail.Subject = Subject;
    //        emaillistTo = MailID;
    //        mail.To.Add(emaillistTo);
    //        mail.IsBodyHtml = true;
    //        mail.Body = message.Body + "<Br /><Br />" + Regards.Replace("@n", "<Br />");
    //        mail.Body = mail.Body + mailmsgDesc.Replace("@n", "<Br />");
    //        mail.Body = mail.Body + CommonConfigurations.Signature;
    //        mail.Body = mail.Body.Replace("@date", DateTime.Now.ToString("dd-MM-yyyy"));
    //        SmtpServer.Port = CommonConfigurations.SmtpPort;
    //        SmtpServer.Credentials = new System.Net.NetworkCredential(senderEmailId, password);
    //        ServicePointManager.Expect100Continue = true;
    //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
    //        SmtpServer.EnableSsl = true;
    //        SmtpServer.Send(mail);
    //        Log.Information("Mail Sent Successfully To MailID: " + MailID);
    //    }
    //    catch (Exception ex)
    //    {
    //        excep = ex.Message + " " + ex.Source + " " + ex.StackTrace;
    //        Log.Error(excep);
    //    }
    //}

    public async Task<string>SendEmailAsync(EmailRequest emailRequest)
    {
        EmailResponse _emailresponse = new EmailResponse();
        var emailSettings = _configuration.GetSection("EmailSettings");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", emailSettings["SmtpUsername"]));
        message.To.Add(new MailboxAddress("Recipient", emailRequest.MailTo));
        message.Cc.Add(new MailboxAddress("Recipient", emailRequest.MailCc));
        // message.Subject = "Transaction Alert: No Transaction in the Last 5 Minutes";
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailRequest.mailbody
        };
        message.Subject = emailRequest.Subject;
        //message.Body = new TextPart(emailRequest.mailbody);
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Connect to the SMTP server asynchronously
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), SecureSocketOptions.StartTls);

                // Authenticate with the SMTP server asynchronously
                await client.AuthenticateAsync(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);

                // Send the email asynchronously
                await client.SendAsync(message);

                // Disconnect from the server
                await client.DisconnectAsync(true);
                _emailresponse.ResponseCode = "success";
                return _emailresponse.ResponseCode;
            }
        }
        catch (SmtpCommandException smtpEx)
        {
            Console.WriteLine($"SMTP command error: {smtpEx.Message}");
            Log.Error(smtpEx.Message);
            _emailresponse.ResponseCode = "Failed";
            return _emailresponse.ResponseCode;// Handle SMTP-specific exceptions
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            excep = ex.Message + " " + ex.Source + " " + ex.StackTrace;
            Log.Error(excep);
            _emailresponse.ResponseCode = "Failed";
            return _emailresponse.ResponseCode;
            // Handle other general exceptions
        }
    }

}
