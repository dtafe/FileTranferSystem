using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using FTSS.Domain;
using FTSS.Utilities;
using Ionic.Zip;
using System.IO;
using System.Threading;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Reflection;


namespace FTSS.Web.Form
{
    public partial class CheckFiles : FTSSPageUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    string FileId = Request.QueryString["ID"];
                    if (FileId == null)
                    {
                        FileId = "0"; // test 
                    }
                    fileId.Text = FileId;

                    InitLanguage();
                    Initialize();

                    getCustomer(FileId);
                    getFile(FileId);
                    getOther(FileId);

                    getCustomerID(FileId);
                    string item = string.Format(GetResource("MSG_WARN_DELETE_FILE"), " All files ");
                    ButtonAllFilesDelete.Attributes["onclick"] = "if(!confirm('" + item + "')){ return false; };";

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error Page_Load ", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_INFO"), ex.Message) + "\");");
            }
        }

        private void Initialize()
        {
            SetButtonText(ButtonAllFilesDelete,  ButtonAllFilesDownload,ButtonBack, ButtonClose);
            SetLabelText(LabelInvitationTo, LabelFiles, lbExpirationDate);
        }

        protected void getOther(string id)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_GetSharing", Conn))
                    {
                        FileInfoMation obj = new FileInfoMation();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                LabelCustomerName.Text = dr.GetString(1);
                                lbDate.Text = setDateFormat(dr.GetDateTime(2).ToShortDateString());
                            }
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }

        #region CustomerGrid
        protected void getCustomer(string id)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_GetCustomer", Conn))
                    {
                        FileInfoMation obj = new FileInfoMation();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);                        
                        gridCustomers.DataSource = cmd.ExecuteReader();
                        gridCustomers.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }
        #endregion
        protected void getCustomerID(string id)
        {
            List<SharingInfo> listobj = new List<SharingInfo>();
            listobj = Sharing.Sharing_GetByAll();
            HiddenFieldCustomerID.Value = listobj.Where(c => c.ID == Convert.ToInt32(id)).FirstOrDefault().customer_id;
        }
        #region UploadFiles Grid
        protected void getFile(string id)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_GetFile", Conn))
                    {
                        FileInfoMation obj = new FileInfoMation();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);                        
                        fileGrid.DataSource = cmd.ExecuteReader();
                        fileGrid.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }

        protected void fileGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {              
                LinkButton lbtDelete = (LinkButton)e.Row.FindControl("LinkButtonDelete");
                lbtDelete.CommandArgument = e.Row.RowIndex.ToString();
                lbtDelete.Attributes.Add("onclick", "javascript:return confirm('" + string.Format(GetResource("MSG_WARN_DELETE_FILE"), e.Row.Cells[2].Text) + "')");
                LinkButton lbtDownload = (LinkButton)e.Row.FindControl("LinkButtonDownload");
                lbtDownload.CommandArgument = e.Row.RowIndex.ToString();
                /*
                foreach (Button button in e.Row.Cells[4].Controls.OfType<Button>())
                {
                    button.Text = GetResource("ButtonDelete");
                    if (button.CommandName == "Delete")
                    {
                        item = string.Format(GetResource("MSG_WARN_DELETE_FILE"), item);
                        button.Attributes["onclick"] = "if(!confirm('" + item + "')){ return false; };";
                    }
                }
                
                foreach (Button button in e.Row.Cells[3].Controls.OfType<Button>())
                {
                    button.Text = GetResource("ButtonDownload");
                }*/
            }
        }
        protected void fileGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = fileGrid.Rows[index];
            string item = "";
            item = Server.HtmlDecode(row.Cells[2].Text);
           
            switch (e.CommandName)
            {
                case "Delete":
                    deleteFile(item);
                    break;
                case "Download":
                    downloadFile(item);
                    break;
            }
        }

        #endregion

        protected void ButtonBack_Click(object sender, EventArgs e)
        {
            Server.Transfer("CheckOwnFile.aspx");
        }

        protected void ButtonClose_Click(object sender, EventArgs e)
        {
            // To clear Query string value
            PropertyInfo isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

            // make collection editable
            isreadonly.SetValue(this.Request.QueryString, false, null);

            // remove
            if (this.Request.QueryString["ID"] != null)
                this.Request.QueryString.Remove("ID");

            Server.Transfer("LoginMember.aspx");
        }

        protected void ButtonAllFilesDownload_Click(object sender, EventArgs e)
        {
            downloadFile(null);
        }

        protected void ButtonAllFilesDelete_Click(object sender, EventArgs e)
        {
            BlobManager blobManager = new BlobManager();
            foreach (GridViewRow row in fileGrid.Rows)
            {               
                string fname = row.Cells[2].Text;
                FileInforMa.File_Delete(fileId.Text, fname);                
            }
            blobManager.DeleteBlobDirectory(HiddenFieldCustomerID.Value);
            ButtonBack_Click(sender, e);
        }


        protected void downloadFile(string fName)
        {            
            try
            {
                string fileName = "";
                string filePath = "";
                string listName = "";
                List<String> names = new List<String>();
                string Email = "";

                ZipFile zip = new ZipFile();
                if (fName == null)
                {
                    foreach (GridViewRow row in fileGrid.Rows)
                    {
                        fileName = row.Cells[2].Text;
                        listName += fileName + ", ";
                        names.Add(fileName);
                    }
                }
                else
                {
                    fileName = fName;
                    listName += fileName + ", ";
                    names.Add(fileName);
                }
                #region User Azure
                if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                {   
                    // Send Mail
                    foreach( GridViewRow row in gridCustomers.Rows)
                    {
                        Email = row.Cells[1].Text;
                    }
                    
                    UserMail mailSystem = new UserMail("A", "A", Email);
                    var listNamenew = listName.Remove(listName.Length - 2);
                    mailSystem.AddParams("{FileName}", listNamenew);
                    string mailBody = GetMailBody("Alert_mail_download.txt");
                    mailSystem.SendEmail(mailSystem, "Download Notice", mailBody);

                    // Download
                    BlobManager blobManager = new BlobManager();
                    foreach (string namenew in names)
                    {                       
                        WebClient client = new WebClient();
                        var s = client.OpenRead(blobManager.GetURi(LabelCustomerName.Text + "/" + namenew));
                        zip.AddEntry(namenew, s);
                    }
                    Response.Clear();
                    Response.ContentType = "application/zip";
                    if(names.Count > 1)
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + LabelCustomerName.Text + GetResource("All_Files") + ".zip");
                    else
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".zip");
                    zip.Save(Response.OutputStream);
                    Response.End();
                }
                #endregion

                #region User WebServer
                else
                {                    
                    string customerFoler = Server.MapPath("~/" + Constant.UPLOAD_STORAGE) + "\\" + LabelCustomerName.Text;
                    foreach(string NameNew in names)
                    {                        
                        filePath = string.Format("{0}\\{1}", customerFoler, NameNew);
                        if (!File.Exists(filePath))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_FILE_DOWNLOAD_FOUND") + "');", true);
                            return;
                        }
                        if (names.Count > 1)
                            zip.AddFile(filePath, LabelCustomerName.Text + GetResource("All_Files"));
                        else
                            zip.AddFile(filePath, NameNew);
                    }                    
                    if (!string.IsNullOrEmpty(listName))
                    {                        
                        foreach (GridViewRow row in gridCustomers.Rows)
                        {
                            Email += row.Cells[1].Text + ";";
                        }
                        Email = Email.Substring(0, Email.Length - 1);
                        UserMail mailSystem = new UserMail("A", "A", Email);
                        listName = listName.Remove(listName.Length - 2);
                        mailSystem.AddParams("{FileName}", listName);
                        string mailBody = GetMailBody("Alert_mail_download.txt");
                        mailSystem.SendEmail(mailSystem, "Download Notice", mailBody);
                        
                        Response.Clear();
                        Response.ContentType = "application/zip";
                        if(names.Count > 1)
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + LabelCustomerName.Text + GetResource("All_Files") + ".zip");
                        else
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".zip");
                        zip.Save(Response.OutputStream);
                        Response.End();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_AT_LEST_ONE_FILE_DOWNLOAD") + "');", true);
                        return;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
                return ;
            }

        }
        protected void deleteFile(string fname)
        {            
            FileInforMa.File_Delete(fileId.Text, fname);
            getFile(fileId.Text);
            BlobManager blobManager = new BlobManager();
            blobManager.DeleteBlob(HiddenFieldCustomerID.Value + "/" + fname);
            if (fileGrid.Rows.Count ==0)
                Server.Transfer("CheckOwnFile.aspx");
          
        }

        protected void fileGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }    
    }
}

