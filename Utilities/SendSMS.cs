using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.AmootSMS;

namespace Utilities
{
    public class SendSMS
    {
        public static void SendMessage(string To, string body)
        {
            var client = new AmootSMS.AmootSMSWebService2SoapClient("AmootSMSWebService2Soap12");

            string UserName = "09397673794";
            string Password = "1274255325";
            DateTime SendDateTime = DateTime.Now;
            string SMSMessageText = body;
            string LineNumber = "public";
            string[] Mobiles = new string[] { To };
            client = new AmootSMS.AmootSMSWebService2SoapClient("AmootSMSWebService2Soap12");

            AmootSMS.SendResult result = client.SendSimple(UserName, Password, SendDateTime, SMSMessageText, LineNumber, Mobiles);

            if (result.Status == AmootSMS.Status.Success)
            {
                var res = result;
            }


        }

        public static void SendOTP(string phone, string Code)
        {
            var client = new AmootSMS.AmootSMSWebService2SoapClient("AmootSMSWebService2Soap12");
            string UserName = "09397673794";
            string Password = "1274255325";
            string Mobile = phone;
            short CodeLength = 5;
            string OptionalCode = Code;

            client = new AmootSMS.AmootSMSWebService2SoapClient("AmootSMSWebService2Soap12");

            AmootSMS.SendOTPResult result = client.SendQuickOTP(UserName, Password, Mobile, CodeLength, OptionalCode);

            if (result.Status == AmootSMS.Status.Success)
            {
                //خروجی
            }
        }

        public static void SendWithPattern(string phone, string name, string code)
        {
            var client = new AmootSMS.AmootSMSWebService2SoapClient("AmootSMSWebService2Soap12");
            string UserName = "09397673794";
            string Password = "1274255325";
            string Mobile = phone;
            int PatternCodeID = 1460;
            string[] PatternValues = new string[] { name, code };

            client = new AmootSMS.AmootSMSWebService2SoapClient("AmootSMSWebService2Soap12");

            AmootSMS.SendResult result = client.SendWithPattern(UserName, Password, Mobile, PatternCodeID, PatternValues);

            if (result.Status == AmootSMS.Status.Success)
            {
                var res = result.Data;
            }
        }
    }
}
