using Microsoft.Extensions.Configuration;

namespace AlertSending.Classes
{
    public class CommonConfigurations
    {

        private readonly IConfiguration _configuration;

        public CommonConfigurations(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string SenderMailID = string.Empty;
        public static string ServerIPAdress = string.Empty;
        public static string SenderMailPassword = string.Empty;
        public static string Signature = string.Empty;
        public static int SmtpPort = 0;
        public static string SMSURL = string.Empty;
        public static string SMSUserID = string.Empty;
        public static string SMSPassword = string.Empty;

        public void LoadConfigurations()
        {
            SenderMailID = _configuration["EmailSettings:SenderMailID"];
            ServerIPAdress = _configuration["EmailSettings:ServerIPAdress"];
            SenderMailPassword = _configuration["EmailSettings:SenderMailPassword"];
            Signature = _configuration["EmailSettings:Signature"];
            SmtpPort = Convert.ToInt32(_configuration["EmailSettings:SmtpPort"]);
        }
    }
}
