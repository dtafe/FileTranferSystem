using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System.Net.Mail;
using System.Net;

namespace FTSS.Utilities
{
    public class UserMail
    {
        readonly static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string strUserId = string.Empty;
        private string strUserName = string.Empty;
        private string strUserAddress = string.Empty;
       
        private Dictionary<string, string> dictParams = new Dictionary<string, string>();
        private List<string> attachments = new List<string>();

        string mailServer = Common.AppSettingKey(Constant.MAIL_SERVER);
        string mailUserName = Common.AppSettingKey(Constant.MAIL_USER_NAME);
        string mailUser = Common.AppSettingKey(Constant.MAIL_USER);
        string mailPwd = Common.AppSettingKey(Constant.MAIL_PWD);
        string mailTimeOut = Common.AppSettingKey(Constant.MAIL_TIME_OUT);
        int mailPort = Convert.ToInt32(Common.AppSettingKey(Constant.MAIL_PORT));
        bool EnableSsl = Common.AppSettingKey(Constant.MAIL_ENABLE_SSL).ToString().ToLower() == "true" ? true : false; 
        public UserMail()
        {
        }
        public UserMail(string strUserId, string strUserName, string strUserAddress)
        {
            this.strUserId = strUserId;
            this.strUserName = strUserName;
            this.strUserAddress = strUserAddress;
        }

        #region Properties
        public string UserId
        {
            get { return this.strUserId;}
            set {  this.strUserId = value; }
        }
        public string UserName
        {
            get  {  return this.strUserName; }
            set  { this.strUserName = value; }
        }

        public string UserAddress
        {
            get {return this.strUserAddress; }
            set { this.strUserAddress = value; }
        }
        public List<string> Attachments
        {
            get { return attachments; }
            set { attachments = value; }
        }

        #endregion Properties

        #region AddParams 
        public void AddParams(string key, string value)
        {
            if (!dictParams.ContainsKey(key))
                dictParams.Add(key, value);
            else
                dictParams[key] = value;
        }
        public bool ContainParam(string param)
        {
            return dictParams.ContainsKey(param);
        }
        public void UpdateValueToParam(string key, string value)
        {
            if (dictParams.ContainsKey(key))
                dictParams[key] = value;
            else
                dictParams.Add(key, value);
        }
        #endregion 

        #region SendMail
        public void SendEmail(UserMail userLogin, UserMail usermail, string strSubject, string strBody)
        {
            List<UserMail> listUserMailTO = new List<UserMail>();
            if(userLogin!=null)
              listUserMailTO.Add(userLogin);
            listUserMailTO.Add(usermail);
            SendEmail(listUserMailTO, null, strSubject, strBody, null);
        }
        public void SendEmail(UserMail usermail, string strSubject, string strBody)
        {
            List<UserMail> listUserMailTO = new List<UserMail>();
            listUserMailTO.Add(usermail);
            SendEmail(listUserMailTO, null, strSubject, strBody, null);
        }
        public void SendEmail(List<UserMail> listUserMailTO, string strSubject, string strBody)
        {
            SendEmail(listUserMailTO, null, strSubject, strBody, null); 
        }
        public void SendEmail(List<UserMail> listUserMailTO, List<UserMail> listUserMailCC, string strSubject, string strMailBody, params string[] filesAttach)
        {
            List<FileStream> listStreamTemp = new List<FileStream>();
            try
            {
                List<MailAddress> listMemberTO = GetListMember(listUserMailTO);
                List<MailAddress> listMemberCC = GetListMember(listUserMailCC);
                MailAddress fromAddress = new MailAddress(mailUser);
                if (listMemberTO != null && listMemberTO.Count > 0)
                {

                    //SmtpClient client = new SmtpClient(mailServer, mailPort);
                    //client.Host = mailServer;
                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.Timeout = Int32.Parse(mailTimeOut);
                    //client.UseDefaultCredentials = false;
                    //client.Credentials = new NetworkCredential(mailUser, mailPwd);
                    //client.EnableSsl = EnableSsl;
                    var client = new SmtpClient
                    {
                        Host = mailServer,
                        Port = mailPort,
                        EnableSsl = EnableSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, mailPwd),
                        Timeout = Convert.ToInt32(mailTimeOut)
                    };

                    MailMessage mail = new MailMessage(fromAddress, listMemberTO[0]);
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    if (strSubject.Contains("\n"))
                    {
                        strSubject = strSubject.Replace("\n", "");
                        strSubject = strSubject.Replace("\r", "");
                    }
                   
                    mail.Subject = strSubject;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    if (this.Attachments != null)
                    {
                        FileStream fs = null;
                        foreach (string strPath in this.Attachments)
                        {
                            fs = new FileStream(strPath, FileMode.Open, FileAccess.Read);
                            listStreamTemp.Add(fs);
                            mail.Attachments.Add(new Attachment(fs, Path.GetFileName(strPath)));
                        }
                    }
                    foreach (string key in dictParams.Keys)
                        strMailBody = strMailBody.Replace(key, dictParams[key]);

                    if (!strMailBody.Contains("<br"))
                    {
                        strMailBody = strMailBody.Replace("\n", "<br/>");
                        strMailBody = strMailBody.Replace("\r", "");
                    }
                    strMailBody = strMailBody.Replace("<script>", "&ltscript&gt");
                    strMailBody = strMailBody.Replace("</script>", "&lt/script&gt");
                    mail.Body = strMailBody;

                    if (filesAttach != null && filesAttach.Length > 0)
                        foreach (string filePath in filesAttach)
                            if (!String.IsNullOrEmpty(filePath))
                                mail.Attachments.Add(new Attachment(filePath));
                    List<string> lstMailAddr = new List<string>();
                    for (int i = 1; i < listMemberTO.Count; i++)
                    {
                        if (!(lstMailAddr.Contains(listMemberTO[i].Address)))
                        {
                            lstMailAddr.Add(listMemberTO[i].Address);
                            mail.To.Add(listMemberTO[i]);
                        }
                    }
                    if (listMemberCC != null)
                    {
                        foreach (MailAddress addrCC in listMemberCC)
                        {
                            if (!(lstMailAddr.Contains(addrCC.Address)))
                            {
                                lstMailAddr.Add(addrCC.Address);
                                mail.CC.Add(addrCC);
                            }
                        }
                    }
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error send mail " , ex);
                throw ex;
            }
            finally
            {
                foreach (FileStream f in listStreamTemp)
                    f.Close();
            }
        }

     

        private List<MailAddress> GetListMember(List<UserMail> listUserMail)
        {
            if (listUserMail == null)
                return null;
            List<MailAddress> result = new List<MailAddress>();

            try
            {
                MailAddress memberAddress = null;
                if (listUserMail != null)
                {
                    foreach (UserMail user in listUserMail)
                    {
                        if (user.UserAddress.Trim().Length > 0)
                        {
                            memberAddress = new MailAddress(user.UserAddress);
                            result.Add(memberAddress);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            return result;
        }
    }
        #endregion 
      
}
