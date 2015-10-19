using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;
using System.Collections;
using System.IO;
using FTSS.Utilities;
using FTSS.Domain;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text;

namespace FTSS.Web.Form
{
    public partial class DownloadCustomer : FTSSPageUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  
     
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    InitLanguage();
                    SetButtonText(ButtonDownload, ButtonClose);
                    SetLabelText(LabelFileDownloadPeriod, LabelFile, LabelWelcome);
                    CheckBoxAll.Text = GetResource("CheckBoxAll");

                    string customerId = Request.QueryString["customerId"];
                    if (customerId == null)
                    {                        
                        Server.Transfer("LoginCustomer.aspx");                        
                    }

                    LabelCustomerName.Text = customerId;
                    List<SharingInfo> listobj = new List<SharingInfo>();
                    listobj = Sharing.Sharing_GetByAll();

                    var list = listobj.Where(c => c.customer_id == customerId && c.Expiration_date >= DateTime.Now.Date).ToList();
                    if(list.Count >0)
                        LabelDate.Text = setDateFormat(list[0].Expiration_date.ToShortDateString());
                    else
                    {
                        Server.Transfer("LoginCustomer.aspx");
                    }
                    HiddenFieldAcount.Value = list[0].dd_member_account;

                    List<FileInfoMation> objList = new List<FileInfoMation>();
                    objList = FileInforMa.File_GetByAll();
                    objList = objList.Where(c => c.File_Sharing_ID == list[0].ID.ToString()).ToList();
                    foreach (FileInfoMation i in objList)
                    {
                        CheckBoxListFile.Items.Add(new ListItem(i.File_Name, i.File_Sharing_ID.ToString()));
                    }
                    this.ButtonDownload.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(ButtonDownload, "").ToString());
                    //this.ButtonClose.Attributes.Add("onclick", "CloseWindow();"); 
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error Page_Load ", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_INFO"), ex.Message) + "\");");
            }
        }

        /// <summary>
        /// Button Download click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        protected void ButtonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonDownload.Enabled = true;
                //Azure
                if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                {                    
                    DownloadAzure();                    
                }
                //Web
                else
                {
                    DownloadWebServer();
                }
            }            
            catch (Exception ex)
            {
                ButtonDownload.Enabled = true; 
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
            }
        }        

        /// <summary>
        /// Download webserver
        /// </summary>
        protected void DownloadWebServer()
        {                   
            string fileName = "";
            string filePath = "";
            string listName = "";
            List<String> names = new List<String>();
            string Email = "";
            string customerFoler = "";
            ZipFile zip = new ZipFile();           
            //Get name
            customerFoler = Server.MapPath("~/" + Constant.UPLOAD_STORAGE) + "\\" + LabelCustomerName.Text;
            for (int i = 0; i < CheckBoxListFile.Items.Count; i++)
            {
                if (CheckBoxListFile.Items[i].Selected)
                {
                    fileName = CheckBoxListFile.Items[i].ToString();
                    listName += fileName + ", ";
                    names.Add(fileName);                           
                }
            }
            // Send Mail and download
            if (!string.IsNullOrEmpty(listName))
            {
                //Send Mail
                List<DDMemberInfoCustomer> listMemberAccount = new List<DDMemberInfoCustomer>();
                listMemberAccount = DDMemberCallControlCustomer.File_GetByAll();
                listMemberAccount = listMemberAccount.Where(c => c.Account == HiddenFieldAcount.Value).ToList();
                Email = listMemberAccount[0].Email;

                UserMail mailSystem = new UserMail("A", "A", Email);
                listName = listName.Remove(listName.Length - 2);
                mailSystem.AddParams("{FileName}", listName);
                string mailBody = GetMailBody("Alert_mail_download.txt");
                mailSystem.SendEmail(mailSystem, "Download Notice", mailBody);

                //Zip file
                foreach(string namenew in names)
                {
                    filePath = string.Format("{0}\\{1}", customerFoler, namenew);
                    if (!File.Exists(filePath))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_FILE_DOWNLOAD_FOUND") + "');", true);
                        return;
                    }
                    else if(names.Count > 1)
                        zip.AddFile(filePath, LabelCustomerName.Text + GetResource("All_Files"));
                    else
                        zip.AddFile(filePath, namenew);
                }
                //Download
                ButtonDownload.Enabled = true;
                logger.Info("start");
                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "application/zip";
                if (names.Count > 1)
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + LabelCustomerName.Text + GetResource("All_Files") + ".zip");
                else
                {
                    fileName = fileName.Remove(fileName.Length - 4);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".zip");
                }
                zip.Save(Response.OutputStream);                
                Response.End();
                logger.Info("end");
            }
            else
            {                
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_AT_LEST_ONE_FILE_DOWNLOAD") + "');", true);
                return;
            }
        }

        /// <summary>
        /// Download Azure
        /// </summary>
        protected void DownloadAzure()
        {
            string fileName = "";            
            string listName = "";
            List<String> names = new List<String>();
            string Email = "";            
            ZipFile zip = new ZipFile();                
            
            // Get Name
            for (int i = 0; i < CheckBoxListFile.Items.Count; i++)
            {
                if (CheckBoxListFile.Items[i].Selected)
                {
                    fileName = CheckBoxListFile.Items[i].ToString();
                    listName += fileName + ", ";
                    names.Add(fileName);
                }
            }
            //Send Mail And download
            if (!string.IsNullOrEmpty(listName))
            {
                //Send Mail
                try
                {
                    List<DDMemberInfoCustomer> listMemberAccount = new List<DDMemberInfoCustomer>();
                    listMemberAccount = DDMemberCallControlCustomer.File_GetByAll();
                    listMemberAccount = listMemberAccount.Where(c => c.Account == HiddenFieldAcount.Value).ToList();
                    Email = listMemberAccount[0].Email;

                    UserMail mailSystem = new UserMail(HiddenFieldAcount.Value, HiddenFieldAcount.Value, Email);
                    var listNamenew = listName.Remove(listName.Length - 2);
                    mailSystem.AddParams("{FileName}", listNamenew);
                    string mailBody = GetMailBody("Alert_mail_download.txt");

                    mailSystem.SendEmail(mailSystem, Common.GetResourceString("MAIL_DownloadSubject"), mailBody);
                }
                catch
                {

                }

                // Download                
                BlobManager blobManager = new BlobManager();
                foreach (string namenew in names)
                {            
                   
                    WebClient client = new WebClient();
                    var s = client.OpenRead(blobManager.GetURi(LabelCustomerName.Text  + "/" + namenew));
                    zip.MaxOutputSegmentSize = 1024 * 1024 * 1024;
                    zip.AddEntry(namenew, s);                     
                }

                ButtonDownload.Enabled = true;
                Response.Clear();
                Response.Buffer = false;
                Response.BufferOutput = false;
                Response.ContentType = "application/zip";
                if (names.Count > 1)
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + LabelCustomerName.Text + GetResource("All_Files") + ".zip");
                else
                {
                    fileName = fileName.Remove(fileName.Length - 4);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".zip");
                }
                Response.Flush();
                zip.Save(Response.OutputStream);
               Response.End();                
            }
            else
            {                
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_AT_LEST_ONE_FILE_DOWNLOAD") + "');", true);
                return;
            }
        }

        protected void ButtonClose_Click(object sender, EventArgs e)
        {
            Server.Transfer("LoginCustomer.aspx"); 
        }
    }
}