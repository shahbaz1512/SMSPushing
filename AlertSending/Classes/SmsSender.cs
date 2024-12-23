namespace AlertSending.Classes
{
    using Serilog;
    using System.Net;
    using System.Threading.Tasks;

    public class SmsSender
    {

        public async Task SendSmsAsync(string SMS, string MobileNumber)
        {
            string URL = string.Empty;
            try
            {
                if ((!string.IsNullOrEmpty(MobileNumber)) && (MobileNumber.Length == 10) && (!string.IsNullOrEmpty(SMS)))
                {
                    URL = CommonConfigurations.SMSURL.Replace("*", "&")
                          .Replace("@Mobile", MobileNumber)
                          .Replace("@Text", SMS)
                          .Replace("@MobileNumber@", MobileNumber)
                          .Replace("@SMS@", SMS)
                          .Replace("@SMSUserID@", CommonConfigurations.SMSUserID)
                          .Replace("@SMSPassword@", CommonConfigurations.SMSPassword)
                          .Replace("@DateTime@", DateTime.Now.ToString("yyyyMMddHHmm"))
                          .Replace("@SenderName@", "");
                    Log.Information(URL);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                    HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                    HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                    String ver = myWebResponse.ProtocolVersion.ToString();
                    StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
                    string strResponse = reader.ReadLine();
                    Log.Information(string.Format("SMS Status : {0} ", strResponse.Replace("\r\n", "")));
                }
                else
                {
                    Log.Information(string.Format("SMS Sending DATA/Mobile Number is Incorrect : {0},\t {1} ", MobileNumber, SMS));
                }
            }
            catch (Exception ex)
            { Log.Error(ex.Message); MobileNumber = string.Empty; SMS = string.Empty; URL = string.Empty; }
        }
    }
}
