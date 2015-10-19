using FTSS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Mail;
using System.Net.Mail;

namespace FTSS.Web.WebService
{
    /// <summary>
    /// Summary description for Test
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Test : System.Web.Services.WebService
    {
        [WebMethod]
        public string TestSendMail(string mailTo)
        {
            try
            {
              
                // If your smtp server requires TLS connection, please add this line
                // oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                // If your smtp server requires implicit SSL connection on 465 port, please add this line
                // oServer.Port = 465;
                // oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;


                return "SSS";
            }
            catch (Exception ex)
            {
                return "Send mail error" + ex.Message;
            }
        }
    }
}
