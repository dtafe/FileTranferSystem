using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Resources;
using System.Reflection;
using FTSS.Utilities;
using System.Text;

namespace FTSS.Web.Form
{
    public partial class LoginMember : FTSSPageUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //ResourceManager FormResources = new ResourceManager("FTSS.Web.Form.LoginMember", Assembly.GetExecutingAssembly());

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                InitLanguage();
                Initialize();
            }
            catch (Exception ex)
            {
                logger.Error("MemberLogin load fail", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
            }
        }

        private void Initialize()
        {
            SetButtonText(ButtonLogin, ButtonCancel);
            SetLabelText(LabelUserAccount, LabelPassword);
            rdbDownloadFile.Text = GetResource("rdbDownloadFile.Text");    //FormResources.GetString("rdbDownloadFile.Text");
            rdbIssueToCustomer.Text = GetResource("rdbIssueToCustomer.Text");
            rdbCheckOwnFile.Text = GetResource("rdbCheckOwnFile.Text");
            lbMode.Text = GetResource("lbMode.Text");
            requiredUser.ToolTip = requiredUser.ErrorMessage = string.Format(GetResource("MSG_REQUIRED_INPUT_FIELD"), LabelUserAccount.Text);
            requiredPassword.ToolTip = requiredPassword.ErrorMessage = string.Format(GetResource("MSG_REQUIRED_INPUT_FIELD"), LabelPassword.Text);
            LinkButtonAdmin.Text = GetResource("LinkButtonManagement");
            LinkButtonUse.Text = GetResource("LinkButtonUse");
        }

        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string encrypt = GenerateHashWithSalt(txtPassword.Text, txtUser.Text);
                using (var con = new SqlConnection(Common.GetConnectString()))
                using (var cmd = new SqlCommand("DDV_GetUserPass", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@account", txtUser.Text);
                    cmd.Parameters.AddWithValue("@pw", encrypt);
                    con.Open();
                    var result = (int)cmd.ExecuteScalar();
                    con.Close();
                    if (result > 0)
                    {
                        Session["user"] = txtUser.Text;

                        if (rdbDownloadFile.Checked)
                            Server.Transfer("Upload.aspx");
                        else if (rdbIssueToCustomer.Checked)
                            Server.Transfer("Upload.aspx?mode=2");
                        else if (rdbCheckOwnFile.Checked)
                            Server.Transfer("CheckOwnFile.aspx");
                    }
                    else
                        RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("lbMsg.Text"), "") + "\");"); // "Login fail";
                }
            }
            catch (Exception ex)
            {
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_INFO"), ex.Message) + "\");");
            }
        }
        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            txtPassword.Text = "";
            txtUser.Text = "";
        }
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
        protected void LinkButtonAdmin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUser.Text) || String.IsNullOrEmpty(txtPassword.Text))
            {
                return;
            }
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    InitLanguage();
                    Initialize();
                    //Decrypt password
                    string encrypt = GenerateHashWithSalt(txtPassword.Text, txtUser.Text);

                    using (SqlCommand cmd = new SqlCommand("DDV_LoginMember", Conn))
                    {
                        Conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@account", txtUser.Text);
                        cmd.Parameters.AddWithValue("@password", encrypt);
                        SqlDataReader reader = cmd.ExecuteReader();

                        bool login = reader.Read();
                        #region Check Login
                        if (login)
                        {
                            string check = CheckPermission(txtUser.Text);
                            switch (check)
                            {
                                case "1":
                                    Session["admin"] = txtUser.Text;
                                    Response.Redirect("ManagerMember.aspx");
                                    break;
                                case "2":
                                    RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"),GetResource("TITLE_PERMISSION")) + "\");");
                                    break;
                                case "":
                                    RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), GetResource("Title_NoActived")) + "\");");
                                    break;
                            }
                        }
                        else
                        {
                            RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), GetResource("Account_Fail")) + "\");");
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Loggin admin page error ", ex);
                }
                finally
                {
                    Conn.Close();
                }
            }
        }
        #region Check Permission
        private string CheckPermission(string input)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("DDV_CheckRole", Conn))
                    {
                        Conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@account", input);
                        string role = Convert.ToString(cmd.ExecuteScalar());
                        return role;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Check admin role fail", ex);
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }
        #endregion
        
    }
}