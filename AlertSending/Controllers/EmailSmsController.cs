using AlertSending.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

[Route("api/[controller]")]
[ApiController]
public class EmailSmsController : Controller
{
    private readonly EmailSender _emailSender;
    private readonly SmsSender _smsSender;
    EmailRequest _emailrequest = new();
    public EmailSmsController(EmailSender emailSender, SmsSender smsSender)
    {
        _emailSender = emailSender;
        _smsSender = smsSender;
    }

    [HttpPost("sendemail")]
    public async Task<IActionResult> SendEmail()
    {
        try
        {
            EmailResponse emailResponse = new EmailResponse();
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody.Trim()))
            {
                Log.Information("Received an empty request body.");
                return BadRequest("Invalid request body.");
            }
            _emailrequest = JsonConvert.DeserializeObject<EmailRequest>(requestBody);
            Log.Information(" Request Recieved For SendEmail ");
            //await _emailSender.SendEmailAsync(_emailrequest.AlertAddress, _emailrequest.MailID, _emailrequest.Subject, _emailrequest.Initials, _emailrequest.Regards, _emailrequest.Bankname, _emailrequest.Attachment, _emailrequest.dsbody, _emailrequest.bodyattachment, _emailrequest.mailmsgDesc);
            //await _emailSender.SendEmailAsync(_emailrequest.AlertAddress, _emailrequest.MailID, _emailrequest.Subject, _emailrequest.Initials, _emailrequest.Regards, _emailrequest.Bankname, _emailrequest.Attachment, _emailrequest.dsbody, _emailrequest.bodyattachment, _emailrequest.mailmsgDesc);
            emailResponse.ResponseCode = await _emailSender.SendEmailAsync(_emailrequest);
            Log.Information(" Response : " + emailResponse.ResponseCode);
            return StatusCode(StatusCodes.Status200OK, emailResponse);
        }
        catch (Exception ex)
        {
            Log.Error("Error sending Email ");
            throw ex;
        }

    }

    [HttpPost("sendsms")]
    public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
    {
        try
        {
            Log.Information(" Request Recieved For SendSms ");
            await _smsSender.SendSmsAsync(request.ToPhoneNumber, request.Message);
            Log.Information(" Response sent successfully. ");
            return Ok("SMS sent Successfully.");
        }
        catch (Exception ex)
        {
            Log.Error("Error sending SendSms. ");
            throw ex;
        }

    }
}

public class EmailRequest
{
    public int id { get; set; }
    public string AlertAddress { get; set; }
    public string MailTo { get; set; }
    public string MailCc { get; set; }
    public string MailBcc { get; set; }
    public string Subject { get; set; }
    public string Initials { get; set; }
    public string Regards { get; set; }
    public string Bankname { get; set; }
    public string Attachment { get; set; }
    public string mailbody { get; set; }
    public string bodyattachment { get; set; }
    public string mailmsgDesc { get; set; }
}
public class SmsRequest
{
    public string ToPhoneNumber { get; set; }
    public string Message { get; set; }
}
public class EmailResponse
{
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
}