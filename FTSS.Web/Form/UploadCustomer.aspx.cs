using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using FTSS.Utilities;
using System.Collections;
using FTSS.Domain;
using System.Data.SqlClient;

namespace FTSS.Web.Form
{
    public partial class UploadCustomer : FTSSPageUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);       
        private Dictionary<string, HttpPostedFile> postedFiles;
        public Dictionary<string, HttpPostedFile> PostedFiles
        {
            get
            {
                return ((Dictionary<string, HttpPostedFile>)Session["PostedFiles"]);
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
                if (!IsPostBack)
                {
                    InitLanguage();
                    Session["PostedFiles"] = new Dictionary<string, HttpPostedFile>();
                    string customerId = Request.QueryString["customerId"];        
                    if (customerId == null)
                    {
                        Server.Transfer("LoginCustomer.aspx");                        
                    }
                    LabelCustomerName.Text = customerId;
                    SetLabelText(LabelbUploadfilelist, LabelWelcome, LabelUploadFile, LabelFile);
                    SetButtonText(ButtonAddFile, ButtonUpload, ButtonClose, ButtonBrowse);
                   

                    List<SharingInfo> listobj = new List<SharingInfo>();
                    listobj = Sharing.Sharing_GetByAll();
                    var list = listobj.Where(c => c.customer_id == customerId && c.Expiration_date >= DateTime.Now.Date).ToList();
                    if (list.Count > 0)
                    {
                        HiddenFieldID.Value = list[0].ID.ToString();
                        HiddenFieldAccount.Value = list[0].dd_member_account;
                    }

                    List<FileInfoMation> listFile = new List<FileInfoMation>();
                    listFile = FileInforMa.File_GetByAll();
                    listFile = listFile.Where(c => c.File_Sharing_ID == HiddenFieldID.Value).ToList();

                    ViewState.Remove("HashCheckArray");
                    ViewState.Add("HashCheckArray", listFile);
                    FillGridview(listFile);                    
                    this.ButtonUpload.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(ButtonUpload, "").ToString());
                    this.ButtonAddFile.Attributes.Add("onclick", "this.disabled=true;" + Page.ClientScript.GetPostBackEventReference(ButtonAddFile, "").ToString());                    
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error Page_Load ", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_INFO"), ex.Message) + "\");");
            }
        }
        /// <summary>
        /// FillGridview
        /// </summary>
        /// <param name="objList"></param>
        protected void FillGridview(List<FileInfoMation> objList)
        {
            objList = objList.OrderBy(c => c.Upload_Date).ToList();
            GridViewlistUpload.Columns[1].Visible = true;
            GridViewlistUpload.DataSource = objList;
            GridViewlistUpload.DataBind();
            GridViewlistUpload.Columns[1].Visible = false;            
        }
        /// <summary>
        /// ButtonAdd Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonAdd_Click(object sender, EventArgs e)
        {  
            try
            {                
                List<FileInfoMation> list = new List<FileInfoMation>();               
                //Check exist file file 
                txtFilePath.Text = ""; 
                if (PostedFiles.ContainsKey(Path.GetFileName(FileUploadSelect.PostedFile.FileName)))
                {
                    //already exist file 
                    PostedFiles.Remove(Path.GetFileName(FileUploadSelect.PostedFile.FileName));
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_SELECTED_FILE_ALREADY_UPLOAD") + "');", true);                            
                    return;                 
                }
                if (string.IsNullOrEmpty(Path.GetFileName(FileUploadSelect.PostedFile.FileName)))
                {
                    //Not file
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_SELECTED_FILE_NOT_EXIST") + "');", true);                          
                    return;                       
                }                
                // pass
                if (!PostedFiles.ContainsKey(Path.GetFileName(FileUploadSelect.PostedFile.FileName)))
                {
                    List<FileInfoMation> listtemp = new List<FileInfoMation>();
                    FileInfoMation obj = new FileInfoMation();                   

                    if (ViewState["HashCheckArray"] != null)
                        list = (List<FileInfoMation>)ViewState["HashCheckArray"];

                    listtemp = list.Where(c => c.File_Name == Path.GetFileName(FileUploadSelect.PostedFile.FileName)).ToList();
                    if (listtemp.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "alert('" + GetResource("MSG_SELECTED_FILE_ALREADY_UPLOAD") + "');", true);
                        return;
                    }

                    if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "" && FileUploadSelect.FileContent.Length >= Constant.MAX_FILE_COMPARE)
                    {
                        string blobName = "Temp/" + HiddenFieldAccount.Value + "/" +   FileUploadSelect.PostedFile.FileName;
                        BlobManager blob = new BlobManager();
                        blob.UploadFromStream(FileUploadSelect.PostedFile.InputStream, blobName);

                        PostedFiles.Add(Path.GetFileName(FileUploadSelect.PostedFile.FileName), null);
                    }
                    else
                    {
                        PostedFiles.Add(Path.GetFileName(FileUploadSelect.PostedFile.FileName), FileUploadSelect.PostedFile);
                    }
                    obj.File_Sharing_ID = HiddenFieldID.Value;
                    obj.File_Name = Path.GetFileName(FileUploadSelect.PostedFile.FileName);
                    obj.File_Size = FileUploadSelect.PostedFile.ContentLength.ToString();
                    obj.Upload_Date = DateTime.Now.ToString();                    
                    list.Add(obj);                         
                    ViewState.Add("HashCheckArray", list);                    
                    FillGridview(list);                          
                }
            }
            catch (Exception ex)
            {
                ButtonAddFile.Enabled = true;
                txtFilePath.Text = ""; 
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
            } 
        }       
        /// <summary>
        /// ButtonUpload Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonUpload_Click(object sender, EventArgs e)
        {            
            Dictionary<string, HttpPostedFile> dicPostedFiles = PostedFiles;
            try 
            { 
                StartUpload(dicPostedFiles);
                List<FileInfoMation> listFile = new List<FileInfoMation>();
                listFile = FileInforMa.File_GetByAll();
                listFile = listFile.Where(c => c.File_Sharing_ID == HiddenFieldID.Value).ToList();
                ViewState.Remove("HashCheckArray");
                ViewState.Add("HashCheckArray", listFile);
                FillGridview(listFile);                
            }
            catch (Exception ex)
            {
                ButtonUpload.Enabled = true;
                if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                {
                    BlobManager blobManager = new BlobManager();
                    foreach (string fileName in dicPostedFiles.Keys)
                    {
                        HttpPostedFile postedFile = dicPostedFiles[fileName];
                        if (postedFile == null)
                        {
                            blobManager.DeleteBlob("Temp/" + HiddenFieldAccount.Value + "/" +  fileName);
                        }
                        else
                            blobManager.DeleteBlob(LabelCustomerName.Text + "/" + fileName);
                    }
                }
                else
                {
                    foreach (HttpPostedFile postedFile in dicPostedFiles.Values)
                    {
                        string directoryPath = Server.MapPath("~/" + Constant.UPLOAD_STORAGE + "\\" + LabelCustomerName.Text.Trim() + "\\" + Path.GetFileName(postedFile.FileName));
                        if (File.Exists(directoryPath))
                            File.Delete(directoryPath);

                    }
                }
                logger.Error("Error btnUpload_Click", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
            }  
        }
        /// <summary>
        /// Upload............
        /// </summary>
        /// <param name="dicPostedFiles"></param>
        private void StartUpload(Dictionary<string, HttpPostedFile> dicPostedFiles)
        {
            string EmailCustomer = "";
            string EmailMember = "";
            string listName = "";
            
            #region Upload file
            if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
            {
                BlobManager blobManager = new BlobManager();
                foreach (string fileName in dicPostedFiles.Keys)
                {
                    HttpPostedFile postedFile = dicPostedFiles[fileName];
                    long ContentLength = 0;
                    if (postedFile == null)
                    {
                        ContentLength = blobManager.CopyBlob("Temp/" + HiddenFieldAccount.Value + "/" +  fileName, LabelCustomerName.Text + "/" + fileName);
                    }
                    else
                    {
                        ContentLength = postedFile.ContentLength;
                        blobManager.UploadFromStream(postedFile.InputStream, LabelCustomerName.Text + "/" + fileName);
                    }

                    listName += fileName + ", ";
                    FileInfoMation obj = new FileInfoMation();
                    obj.File_Sharing_ID = HiddenFieldID.Value;
                    obj.File_Name = Path.GetFileName(fileName);
                    obj.File_Size = ContentLength.ToString();
                    obj.Upload_Date = DateTime.Now.ToString();
                    FileInforMa.File_Insert(obj);
                }
            }
            else
            {
                foreach (HttpPostedFile postedFile in dicPostedFiles.Values)
                {
                    CreateFolder();
                    string directoryPath = Server.MapPath("~/" + Constant.UPLOAD_STORAGE + "\\" + LabelCustomerName.Text.Trim());
                    postedFile.SaveAs(directoryPath + "\\" + Path.GetFileName(postedFile.FileName));

                    listName += Path.GetFileName(postedFile.FileName) + ", ";
                    FileInfoMation obj = new FileInfoMation();
                    obj.File_Sharing_ID = HiddenFieldID.Value;
                    obj.File_Name = Path.GetFileName(postedFile.FileName);
                    obj.File_Size = postedFile.ContentLength.ToString();
                    obj.Upload_Date = DateTime.Now.ToString();
                    FileInforMa.File_Insert(obj);
                }
                
            }
            #endregion

            #region SendMail
            if (!string.IsNullOrEmpty(listName))
            {
                listName = listName.Remove(listName.Length - 2);
                ///// send mail customer 
                List<CustomerInfo> listCustomer = new List<CustomerInfo>();
                listCustomer = CustomerCallControl.File_GetByAll();
                listCustomer = listCustomer.Where(c => c.File_Sharing_ID == HiddenFieldID.Value).ToList();
                EmailCustomer = listCustomer[0].CustomerEmail;

                UserMail mailSystemCus = new UserMail(HiddenFieldAccount.Value, HiddenFieldAccount.Value, EmailCustomer);
                mailSystemCus.AddParams("{FileName}", listName);
                string mailBody = GetMailBody("Alert_mail_upload.txt");
                mailSystemCus.SendEmail(mailSystemCus, Common.GetResourceString("MAIL_UploadSubject"), mailBody);                
                
                ///// send mail member
                List<DDMemberInfoCustomer> listMemberAccount = new List<DDMemberInfoCustomer>();
                listMemberAccount = DDMemberCallControlCustomer.File_GetByAll();
                listMemberAccount = listMemberAccount.Where(c => c.Account == HiddenFieldAccount.Value).ToList();
                EmailMember = listMemberAccount[0].Email;

                UserMail mailSystemMem = new UserMail(HiddenFieldAccount.Value, HiddenFieldAccount.Value, EmailMember);
                mailSystemMem.AddParams("{FileName}", listName);
                mailSystemMem.SendEmail(mailSystemMem, Common.GetResourceString("MAIL_UploadSubject"), mailBody);
                PostedFiles = new Dictionary<string, HttpPostedFile>();
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_SUCESS"), GetResource("MSG_UPLOAD_SUCESS")) + "\");");
            }
            else               
                throw new Exception(GetResource("MSG_AT_LEAST_ONE_CUSTOMER"));            
            #endregion
           
        }
        /// <summary>
        /// GridViewlistUpload RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewlistUpload_RowDataBound(object sender, GridViewRowEventArgs e)
        {            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton l = (LinkButton)e.Row.FindControl("LinkButton1");
                string messageConfirm = string.Format(GetResource("MSG_WARN_DELETE_FILE"), e.Row.Cells[2].Text);
                l.Attributes.Add("onclick", "javascript:return confirm('" + messageConfirm + "')");
            }
        }
        /// <summary>
        /// Delete event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewlistUpload_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string count = GridViewlistUpload.Rows[e.RowIndex].Cells[0].Text.ToString();
            string id = GridViewlistUpload.Rows[e.RowIndex].Cells[1].Text.ToString();
            string name = GridViewlistUpload.Rows[e.RowIndex].Cells[2].Text.ToString();
            
            List<FileInfoMation> list = new List<FileInfoMation>();
            List<FileInfoMation> listOld = new List<FileInfoMation>();
            List<FileInfoMation> listtemp = new List<FileInfoMation>();

            list = (List<FileInfoMation>)ViewState["HashCheckArray"];
            listOld = FileInforMa.File_GetByAll();
            listOld = listOld.Where(c => c.File_Sharing_ID == id).ToList();
            //check value data  
            listtemp = listOld.Where(c => c.File_Name == name).ToList();
            if (listtemp.Count > 0)
            {
                FileInforMa.File_Delete(id, name);
                if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                {
                    BlobManager blobManger = new BlobManager();
                    blobManger.DeleteBlob(LabelCustomerName.Text + "/" + name); 
                }
                else
                {
                    string customerFoler = Path.Combine(GetMapPath() + Constant.UPLOAD_STORAGE + "\\" + LabelCustomerName.Text);
                    if (File.Exists(customerFoler + "\\" + name))
                    {
                        File.Delete(customerFoler + "\\" + name);
                    }
                }
            }
            else if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
            {
                BlobManager blobManger = new BlobManager();
                blobManger.DeleteBlob("Temp/" + HiddenFieldAccount.Value + "/" +  name);
            }
          
            PostedFiles.Remove(name);            
            list = list.Where(c => c.File_Name != name).ToList();

            ViewState.Remove("HashCheckArray");
            ViewState["HashCheckArray"] = list;            
            FillGridview(list);
        }       
        protected void ButtonClose_Click(object sender, EventArgs e)
        {
            ViewState.Remove("HashCheckArray");
            Session.Remove("PostedFiles");
            Response.Redirect("LoginCustomer.aspx");
        }
        protected void CreateFolder()
        {            
            string directoryPath = Server.MapPath("~/" + Constant.UPLOAD_STORAGE + "\\" + LabelCustomerName.Text.Trim());            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }        
    }
}