using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using FTSS.Domain;
using FTSS.Utilities;

namespace FTSS.Web.Form
{
    public partial class Upload : FTSSPageUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<CustomerInfo> CustomerInfo
        {
            get
            {
                if (this.ViewState["CustomerInfo"] == null)
                {
                    this.ViewState["CustomerInfo"] = new List<CustomerInfo>();
                }
                return this.ViewState["CustomerInfo"] as List<CustomerInfo>;
            }
        }
        public class DetailsFile //Class for binding data
        {
            public string FileName { get; set; }
            public int ContentLength { get; set; }
        }
        private List<DetailsFile> postedFiles;
        public List<DetailsFile> PostedFiles
        {
            get
            {
                return ((List<DetailsFile>)Session["PostedFiles"]);
            }
            set
            {
                Session["PostedFiles"] = value;
                postedFiles = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                hiddenAccount.Value = "";
                if (Session["user"] == null)
                    Server.Transfer("LoginMember.aspx");

                hiddenAccount.Value = Session["user"].ToString();

                if (PostedFiles == null)
                    PostedFiles = new List<DetailsFile>();
                #region Sava file to temp folder
                foreach (string filename in Request.Files)
                {
                    HttpPostedFile file = Request.Files.Get(filename);

                    if (file != null && file.ContentLength > 0)
                    {
                        DetailsFile objFile = new DetailsFile();
                        objFile.FileName = file.FileName;
                        objFile.ContentLength = file.ContentLength;
                        PostedFiles.Add(objFile);
                        string blobName = "Temp/" + hiddenAccount.Value + "/" + file.FileName;

                        BlobManager blob = new BlobManager();
                        blob.UploadFromStream(file.InputStream, blobName);
                    }
                }
                #endregion

                #region IsPostback
                if (!Page.IsPostBack)
                {
                    
                    BindGridView();
                    InitLanguage();

                    SetLabelText(LabelCustomerInfo, LabelCustomerName, LabelEmailAddress, LabelCustomerList, LabelTermUpload, LabelbUploadfilelist, LabelDay, lblDropdrag);
                    SetButtonText(ButtonAddCustomer, ButtonUpload, ButtonCancel);
                    requiredName.ToolTip = requiredName.ErrorMessage = string.Format(GetResource("MSG_REQUIRED_INPUT_FIELD"), LabelCustomerName.Text);
                    requiredEmail.ToolTip = requiredEmail.ErrorMessage = string.Format(GetResource("MSG_REQUIRED_INPUT_FIELD"), LabelEmailAddress.Text);
                    validateEmail.ToolTip = validateEmail.ErrorMessage = GetResource("MSG_VALID_EMAIL");
                    //requiredDetailsFile.ToolTip = requiredDetailsFile.ErrorMessage = GetResource("MSG_SELECTED_FILE_NOT_EXIST");
                    for (int i = 1; i <= 10; i++)
                        dropdownTermUpload.Items.Add(i.ToString());

                    string mode = Request.QueryString["mode"]; // mode=2 Invitation Upload
                    if (mode != null)
                    {
                        panelUploadFiles.Visible = false;
                        ButtonUpload.Text = GetResource("ButtonInviteUpload");
                    }
                    else
                    {
                        ButtonUpload.Text = GetResource("ButtonUploadSend");
                    }
                    this.ButtonUpload.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(ButtonUpload, "").ToString());

                #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error Page_Load ", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_INFO"), ex.Message) + "\");");
            }
        }

        #region CustomerGrid
        protected void gridCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowState != DataControlRowState.Edit) // check for RowState
            {
                if (e.Row.RowType == DataControlRowType.DataRow) //check for RowType
                {
                    string messageConfirm = string.Format(GetResource("MSG_WARN_DELETE_FILE"), e.Row.Cells[1].Text); // Get the id to be deleted
                    LinkButton lb = (LinkButton)e.Row.FindControl("LinkButton1");  //(LinkButton)e.Row.Cells[3].Controls[0];
                    if (lb != null)
                    {
                        lb.Attributes.Add("onclick", "return ConfirmOnDelete('" + messageConfirm + "');");
                    }
                }
            }
        }
        protected void gridCustomers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string email = (string)gridCustomers.DataKeys[e.RowIndex].Value;
            DeleteRecordByEmail(email);
            BindGridView();
        }
        private void DeleteRecordByEmail(string email)
        {
            List<CustomerInfo> list = this.CustomerInfo;
            list.RemoveAll(p => p.CustomerEmail == email);
        }
        private void BindGridView()
        {
            BindGridView(this.CustomerInfo);
        }
        private void BindGridView(List<CustomerInfo> list)
        {
            this.gridCustomers.DataSource = list;
            this.gridCustomers.DataBind();
        }
        protected void btnAddCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                this.CustomerInfo.Add(new CustomerInfo(txtName.Text, txtEmail.Text));
                BindGridView();
                txtName.Text = txtEmail.Text = "";
            }
            catch (Exception ex)
            {
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
            }
        }
        #endregion

        #region delete file from folder temp
        [WebMethod(EnableSession = true)]
        public static void DeleteFile(string filename)
        {
            string account = (string)HttpContext.Current.Session["user"].ToString();
            BlobManager blob = new BlobManager();
            blob.DeleteBlob("Temp/" + account + "/" + filename);
            //new Upload<DetailsFile>().PostedFiles.Remove(filename);
        }
        #endregion

        #region Events
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string userLogin = hiddenAccount.Value;
                logger.Info("Begin btnUpload_Click userLogin = " + userLogin);
                string modeCode = panelUploadFiles.Visible ? "0" : "1";
                DateTime expireDate = DateTime.Now.Date.AddDays(Convert.ToInt32(dropdownTermUpload.SelectedValue));
                //1. check Customer list 
                if (this.CustomerInfo.Count == 0)
                    throw new Exception(GetResource("MSG_AT_LEAST_ONE_CUSTOMER"));

                //1.2 Check At least ONE upload file 
                if (panelUploadFiles.Visible == false || PostedFiles.Count == 0)
                    throw new Exception(GetResource("MSG_AT_LEST_ONE_FILE_UPLOAD"));

                //2.Update Database
                string hostPath = GetMapPath();
                List<UserMail> listMailSystem = new List<UserMail>();
                using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
                {
                    SqlTransaction Trans = null;
                    try
                    {
                        Conn.Open();
                        UserMail accountMailLogin = null;
                        using (SqlCommand cmd = new SqlCommand("DDV_GetDDMemberEmail", Conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Account", userLogin);
                            var emailAdress = cmd.ExecuteScalar();
                            if (emailAdress != null)
                                accountMailLogin = new UserMail(userLogin, userLogin, emailAdress.ToString());
                        }
                        logger.Debug("Begin Transaction for update db");
                        Trans = Conn.BeginTransaction();

                        #region Update Database , files

                        foreach (CustomerInfo customer in this.CustomerInfo)
                        {
                            customer.CustomerId = KeyGenerator.GetUniqueKey(20);
                            string customerPass = KeyGenerator.GetUniqueKey(10);
                            string fileSharingId = "";
                            using (SqlCommand cmd = new SqlCommand("DDV_InsertSharingInfo", Conn, Trans))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                                cmd.Parameters.AddWithValue("@dd_member_account", userLogin);
                                cmd.Parameters.AddWithValue("@Mode_code", modeCode);
                                cmd.Parameters.AddWithValue("@customer_id", customer.CustomerId);
                                cmd.Parameters.AddWithValue("@customer_password", customerPass);
                                cmd.Parameters.AddWithValue("@expiration_date", expireDate);

                                cmd.ExecuteNonQuery();
                                fileSharingId = cmd.Parameters["@id"].Value.ToString();
                            }
                            using (SqlCommand cmd = new SqlCommand("DDV_InsertCustomerInfo", Conn, Trans))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@File_Sharing_ID", fileSharingId);
                                cmd.Parameters.AddWithValue("@Customer_name", customer.CustomerName);
                                cmd.Parameters.AddWithValue("@Customer_Email", customer.CustomerEmail);
                                cmd.ExecuteNonQuery();
                            }

                            //Case Upload File
                            string listFileName = "";
                            if (panelUploadFiles.Visible)
                            {
                                logger.Debug("Upload files in customerID =  " + customer.CustomerId);
                                if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                                {
                                    BlobManager blobManager = new BlobManager();

                                    foreach (DetailsFile objfile in PostedFiles)
                                    {
                                        string blobName = customer.CustomerId + "/" + objfile.FileName;
                                        string soureBlob = "Temp/" + hiddenAccount.Value + "/" + objfile.FileName;
                                        
                                        long contentLenght =  blobManager.CopyBlob(soureBlob, blobName);

                                        if (contentLenght > 0)
                                        {
                                            listFileName += objfile.FileName + ",";

                                            using (SqlCommand cmd = new SqlCommand("DDV_InsertFileInfo", Conn, Trans))
                                            {
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@File_Sharing_ID", fileSharingId);
                                                cmd.Parameters.AddWithValue("@File_Name", Path.GetFileName(objfile.FileName));
                                                cmd.Parameters.AddWithValue("@File_Size", objfile.ContentLength);
                                                cmd.Parameters.AddWithValue("@Upload_Date", DateTime.Now.Date);
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string pathCustomerID = CreateUploadCustomerID(hostPath, customer.CustomerId);
                                    foreach (DetailsFile postedFile in PostedFiles)
                                    {

                                        listFileName += postedFile.FileName + "\n";
                                        using (SqlCommand cmd = new SqlCommand("DDV_InsertFileInfo", Conn, Trans))
                                        {
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@File_Sharing_ID", fileSharingId);
                                            cmd.Parameters.AddWithValue("@File_Name", postedFile.FileName);
                                            cmd.Parameters.AddWithValue("@File_Size", postedFile.ContentLength);
                                            cmd.Parameters.AddWithValue("@Upload_Date", DateTime.Now.Date);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            //Add Mail System 
                            UserMail mailSystem = new UserMail(customer.CustomerName, customer.CustomerName, customer.CustomerEmail);
                            mailSystem.AddParams("{DD_MEMBER_NAME}", userLogin);
                            mailSystem.AddParams("{customer_id}", customer.CustomerId);
                            mailSystem.AddParams("{customer_Password}", customerPass);
                            mailSystem.AddParams("{TD_FILE_SHARING_INFORATION.ID}", fileSharingId);
                            mailSystem.AddParams("{TD_FILE_SHARING_INFORMATION.Expiration_date}", expireDate.ToShortDateString());
                            mailSystem.AddParams("{UrlPortal}", Common.AppSettingKey(Constant.PORTAL_URL));
                            if (listFileName.Length > 0)
                                mailSystem.AddParams("{File_Name}", listFileName.Remove(listFileName.Length - 1, 1));
                            listMailSystem.Add(mailSystem);
                        }

                        Trans.Commit();
                        Conn.Close();
                        PostedFiles = null;
                        #endregion

                        #region SendMail
                        logger.Debug("Begin send Email ");
                        string mailBody = "";
                        string subject = "";
                        if (panelUploadFiles.Visible)
                        {
                            mailBody = GetMailBody(hostPath, "Invitation_mail_body_download.txt");
                            subject = Common.GetResourceString("MAIL_DownloadSubject");
                        }
                        else
                        {
                            mailBody = GetMailBody(hostPath, "Invitation_mail_body_upload.txt");
                            subject = Common.GetResourceString("MAIL_InvitationSubject");
                        }
                        foreach (UserMail mail in listMailSystem)
                        {
                            mail.SendEmail(accountMailLogin, mail, subject, mailBody);
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        Conn.Close();
                        try
                        {
                            if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                            {
                                BlobManager blobManager = new BlobManager();
                                foreach (CustomerInfo customer in this.CustomerInfo)
                                {
                                    blobManager.DeleteBlobDirectory(customer.CustomerId);
                                }
                            }
                            else
                            {
                                foreach (CustomerInfo customer in this.CustomerInfo)
                                {
                                    string pathCustomerID = System.IO.Path.Combine(hostPath, Constant.UPLOAD_STORAGE + "\\" + customer.CustomerId);
                                    if (Directory.Exists(pathCustomerID))
                                        Directory.Delete(pathCustomerID, true);
                                }
                            }
                        }
                        catch { logger.Error("Error when try to delete storage", ex); }
                        throw ex;
                    }

                }

                logger.Debug("End btnUpload_Click , loadData()");

                ScriptManager.RegisterStartupScript(this, typeof(Page), "Redirect", "alert(\"" + GetJSMessage(GetResource("TITLE_SUCESS"), GetResource("MSG_UPLOAD_SUCESS")) + "\");window.location='Upload.aspx';", true);
            }
            catch (Exception ex)
            {
                ButtonUpload.Enabled = true;
                logger.Error("Error btnUpload_Click", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
            }
        }
        #endregion

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["PostedFiles"] != null)
                HttpContext.Current.Session.Remove("PostedFiles");
            Response.Redirect("LoginMember.aspx");
        }
    }
}
