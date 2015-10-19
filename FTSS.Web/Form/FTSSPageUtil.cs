using FTSS.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FTSS.Web.Form
{
    public class FTSSPageUtil : System.Web.UI.Page
    {
        protected void InitLanguage()
        {
            HttpCookie cookie = Request.Cookies["CurrentLanguage"];
            if (cookie != null && cookie.Value != null)
            {
                Page.UICulture = cookie.Value;
            }
        }

        protected string setDateFormat(string datetime)
        {

            if (Page.UICulture == "Japanese (Japan)")
            {
                return Convert.ToDateTime(datetime).ToString("yyyy/MM/dd");
            }
            else
            {
                return Convert.ToDateTime(datetime).ToShortDateString();
            }
        }

        protected string GetResource(string resourceName)
        {
            return Common.GetResourceString(resourceName);
        }
        protected void SetLabelText(params Label[] ctrl)
        {
            foreach (Label i in ctrl)
            {
                i.Text = GetResource(i.ID);
            }
        }
        protected void SetButtonText(params Button[] ctrl)
        {
            foreach (Button i in ctrl)
            {
                i.Text = GetResource(i.ID);
            }
        }
        public string GetMapPath()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath);

        }
        public string GetMailBody(string fileMailTemplate)
        {
            return GetMailBody(GetMapPath(), fileMailTemplate);
        }
        public string GetMailBody(string hostPath, string fileMailTemplate)
        {
            string body = "";
            string fullPathTemPlate = System.IO.Path.Combine(hostPath, Constant.MAIL_TEMPLATE + "\\" + fileMailTemplate);
            if (File.Exists(fullPathTemPlate))
                body = File.ReadAllText(fullPathTemPlate, System.Text.Encoding.Default);
            return body;
        }
        public string CreateUploadCustomerID(string customerId)
        {
            return CreateUploadCustomerID(GetMapPath(), customerId);
        }
        public string CreateUploadCustomerID(string hostPath, string customerId)
        {
            string pathCustomerID = System.IO.Path.Combine(hostPath, Constant.UPLOAD_STORAGE + "\\" + customerId);
            if (Directory.Exists(pathCustomerID))
                Directory.Delete(pathCustomerID, true);
            Directory.CreateDirectory(pathCustomerID);
            return pathCustomerID;
        }
        protected void RegisterStartupScript(string script)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 10; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            var key = builder.ToString() + DateTime.Now.Millisecond.ToString();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        protected string GetJSMessage(string title, string message)
        {
            if (string.IsNullOrEmpty(title)) title = string.Empty;
            if (string.IsNullOrEmpty(message)) message = string.Empty;
            return title + "\\n" + ChangeBreakLine((message.Replace('\r', ' ')).Replace("\n", "\\n"));
        }

        protected string ChangeBreakLine(string msg)
        {
            return msg.Replace('\r', ' ').Replace("\n", "\\n").Replace("\\\"", "\"").Replace("\"", "\\\"");
        }

    }
}