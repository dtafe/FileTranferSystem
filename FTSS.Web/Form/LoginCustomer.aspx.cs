using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using FTSS.Utilities;
using FTSS.Domain;
using System.Threading;
namespace FTSS.Web.Form
{
    public partial class LoginCustomer : FTSSPageUtil
    {
        private static SqlConnection cnn = new SqlConnection(Common.GetConnectString());
        
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session.Remove("user");
                Session.Remove("customer");
                InitLanguage();
                Initialize();
            }
            catch(Exception ex)
            {
                logger.Error("load page fail",ex);
                //if (logger.IsErrorEnabled) logger.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + "() Error.", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
            }
        }

        private void Initialize()
        {
            SetButtonText(ButtonLogin);
            SetLabelText(LabelLoginID, LabelPassword, LabelTitleCustomerLogin);
            Val_RequiredLoginID.ErrorMessage = GetResource("Val_RequiredLoginID");
            Val_RequiredLoginPass.ErrorMessage = GetResource("Val_RequiredLoginPass");
            //Session.RemoveAll();
        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtLoginCustomerID.Text == string.Empty || txtLoginCustomerPass.Text == string.Empty)
                {
                    return;
                }
                LoginCustomerInfo objLoginCustomer = new LoginCustomerInfo();
                objLoginCustomer = getCustomerLoginInfo(txtLoginCustomerID.Text, txtLoginCustomerPass.Text);
                if(objLoginCustomer ==null)
                {
                    RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + GetResource("MSG_LOGIN_CUSTOMER_WRONG") + "") + "\");");
                    return;
                }
                if(objLoginCustomer.Delete_flag=="True"||objLoginCustomer.Expiration_date.Date<DateTime.Today.Date)
                {
                    RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + GetResource("MSG_LOGIN_CUSTOMER_INVALID") + "") + "\");");
                    return;
                }
                else
                {
                    Session["customer"] = objLoginCustomer.Customer_id;
                    if (objLoginCustomer.Mode_code == 1)
                    {
                        Server.Transfer("UploadCustomer.aspx?customerId="+objLoginCustomer.Customer_id+"");
                    }
                    else
                    {
                        Server.Transfer("DownloadCustomer.aspx?customerId=" + objLoginCustomer.Customer_id + "");
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
            catch(Exception ex)
            {
                logger.Error("Login custmer fail",ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
            }
        }

        private LoginCustomerInfo getCustomerLoginInfo(string ID, string Pass)
        {
            try
            {
                LoginCustomerInfo objCustomerInfo = new LoginCustomerInfo();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DDV_GetLoginCustomerInfo";
                cmd.Parameters.Add(new SqlParameter("@customer_id", ID));
                cmd.Parameters.Add(new SqlParameter("@customer_password", Pass));
                cnn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter adt = new SqlDataAdapter(cmd);
                adt.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    objCustomerInfo = null;
                }
                else
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        objCustomerInfo.Customer_id = r["customer_id"].ToString();
                        objCustomerInfo.Customer_password = r["customer_password"].ToString();
                        objCustomerInfo.Mode_code = int.Parse(r["Mode_code"].ToString());
                        objCustomerInfo.Delete_flag = r["delete_flag"].ToString();
                        objCustomerInfo.Expiration_date = DateTime.Parse(r["Expiration_date"].ToString());
                    }
                }
                return objCustomerInfo;
            }
            catch(Exception ex)
            {
                logger.Error("Load Database fail ",ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
                return null;
            }
            finally
            {
                cnn.Close();
            }
        }
       
    }
}