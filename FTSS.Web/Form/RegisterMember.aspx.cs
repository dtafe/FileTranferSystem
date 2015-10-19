using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FTSS.Domain;
using FTSS.Utilities;

namespace FTSS.Web.Form
{
    public partial class RegisterMember : FTSSPageUtil
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection
                                                 .MethodBase.GetCurrentMethod().DeclaringType);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitLanguage();
                SetButtonText(ButtonRegister, ButtonCancel);
                SetLabelText(LabelAccount, LabelPassword, LabelName, LabelRePassword, LabelEmail, LabelRegister);
            }
        }
        #region Encrypt
        public static string GenerateHashWithSalt(string password, string salt)
        {
            string sHashWithSalt = password + salt;
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
            // use hash algorithm to compute the hash
            System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();
            // convert merged bytes to a hash as byte array
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            return Convert.ToBase64String(hash);
        }
        #endregion
        protected void buttonRegister_Click(object sender, EventArgs e)
        {
            if (ModelState.IsValid)
            {
                List<UserMail> listMailAdmin = new List<UserMail>();
                using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
                {
                    try
                    {
                        string account = TextboxAccount.Text;
                        string encrypt = GenerateHashWithSalt(TextboxPassword.Text, TextboxAccount.Text);
                        string email = TextboxEmail.Text;
                        string name = TextboxName.Text;
                        
                        Conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DDV_RegisterMember", Conn))
                        {
                            
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@account", account);
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@password", encrypt);                           
                            var result=(int)cmd.ExecuteNonQuery();
                           
                            if (result > 0)
                            {
                                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("RegisterSuccess"), "") + "\");");
                                logger.Info("Register account" + account);
                                SendmailToAdmin();
                                
                            }
                            else
                            {
                                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("RegisterFail"), "") + "\");");
                            }
                        }
                        Conn.Close();
                        
                    }
                        
                    catch (SqlException ex)
                    {
                        logger.Error("Register page error ", ex);
                        RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("AccountExists"),"") + "\");");
                    }
                }

            }
            ResetTextBox();
        }
        public void SendmailToAdmin()
        {
            List<UserMail> listMailAdmin = new List<UserMail>();
            UserMail userMail = null;
            string permission = "1";
            string admin="admin";
            if (admin.ToLower() == "admin")
            {
                using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
                {
                    using (SqlCommand cmd = new SqlCommand("DDV_GetAccountAdmin", Conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@account", admin);
                        Conn.Open();
                        var accountAdmin = cmd.ExecuteScalar();
                        Conn.Close();
                        if (accountAdmin != null)
                        { 
                            userMail= new UserMail(admin, admin, accountAdmin.ToString());
                            string subject = Common.GetResourceString("AccountRegister");
                            string mailBody = GetMailBody("Notify_mail_register.txt");
                            listMailAdmin.Add(userMail);
                            foreach (UserMail mail in listMailAdmin)
	                        {
		                        mail.SendEmail(userMail, mail, subject, mailBody);
	                        }
                        }
                    }
                }
            }
            else
            {
                using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
                {
                    using (SqlCommand cmd = new SqlCommand("DDV_GetEmailAdmin", Conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@permission", permission);
                        Conn.Open();
                        var emailAdmin = cmd.ExecuteScalar();
                        Conn.Close();
                        if (emailAdmin != null)
                        {
                        
                            userMail = new UserMail(permission, permission, emailAdmin.ToString());
                            string subject = Common.GetResourceString("AccountRegister");
                            string mailBody = GetMailBody("Notify_mail_register.txt");
                            listMailAdmin.Add(userMail);
                            foreach (UserMail mail in listMailAdmin)
                            {
                                mail.SendEmail(userMail, mail, subject, mailBody);
                            }
                        }
                    }
                }
            }
        }
       
        private void ResetTextBox()
        {
            TextboxAccount.Text = "";
            TextboxPassword.Text = "";
            TextboxName.Text = "";
            TextboxEmail.Text = "";
            TextboxRePassword.Text = "";
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginMember.aspx");
        }
    }
}