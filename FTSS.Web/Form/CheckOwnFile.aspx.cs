using FTSS.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;
using FTSS.Domain;
using System.Threading;
namespace FTSS.Web.Form
{
    public partial class CheckOwnFile : FTSSPageUtil
    {
        private static SqlConnection cnn = new SqlConnection(Common.GetConnectString());
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string _DD_MemberAccount = string.Empty;

        ResourceManager FormResources = new ResourceManager("FTSS.Web.Form.CheckOwnFileResource", Assembly.GetExecutingAssembly());
       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                InitLanguage();
               
                logger.Info("Starting load check own file page.");
                Initialize();

                if (!IsPostBack)
                {
                    BindingGridView();
                }
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
            catch(Exception ex)
            {
                logger.Error("Load page fail",ex);
                //if (logger.IsErrorEnabled) logger.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + "() Error.", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
            }
        }

        private void BindingGridView()
        {
            try
            {
                List<CheckOwnFilesInfo> listCheckOwnFiles = new List<CheckOwnFilesInfo>();
                listCheckOwnFiles = getCheckOwnFilesInfo(_DD_MemberAccount);
                if (listCheckOwnFiles != null)
                {
                    if (!CheckBoxShowDelete.Checked)
                    {
                        listCheckOwnFiles = listCheckOwnFiles.Where(l => l.delete_flag == "False" || l.delete_flag == "").ToList();
                    }
                }
                if (listCheckOwnFiles == null)
                {
                    CheckOwnFilesInfo objCheckOwnFile = new CheckOwnFilesInfo();
                    List<CheckOwnFilesInfo> list = new List<CheckOwnFilesInfo>();
                    objCheckOwnFile.ID = -1;
                    objCheckOwnFile.Create_date = DateTime.Now;
                    objCheckOwnFile.Customer_name = "";
                    objCheckOwnFile.Customer_Email = "";
                    objCheckOwnFile.Mode_code = -1;
                    objCheckOwnFile.File_name = "";
                    objCheckOwnFile.File_size = "";
                    objCheckOwnFile.Expiration_date = DateTime.Now;
                    objCheckOwnFile.delete_flag = "";
                    list.Add(objCheckOwnFile);

                    gridCheckOwnFile.DataSource = list;
                    gridCheckOwnFile.DataBind();
                    if (list.Count == 1 && list[0].ID == -1)
                    {
                        LabelNoRecord.Visible = true;
                        gridCheckOwnFile.Rows[0].Visible = false;
                        return;
                    }
                }
                else
                {
                    gridCheckOwnFile.DataSource = listCheckOwnFiles;
                    gridCheckOwnFile.DataBind();
                    foreach (GridViewRow r in gridCheckOwnFile.Rows)
                    {
                        if (r.Cells[4].Text == "0")
                            r.Cells[4].Text = GetResource("TextCustomerDownload");
                        else
                            r.Cells[4].Text = GetResource("TextCustomerUpload");
                        if (r.Cells[8].Text == "True")
                            r.Cells[8].Text = GetResource("ColDelete");
                        else
                            r.Cells[8].Text = "";
                        r.Cells[6].Text = Common.ConvertBytesToDisplayText(long.Parse(r.Cells[6].Text));

                        //if (ck1.Value == "ja-JP")
                        //{
                        //    r.Cells[1].Text = Convert.ToDateTime(r.Cells[1].Text).ToString("yyyy/MM/dd");
                        //    r.Cells[7].Text = Convert.ToDateTime(r.Cells[7].Text).ToString("yyyy/MM/dd");
                        //}
                        //else
                        //{
                        //    r.Cells[1].Text = Convert.ToDateTime(r.Cells[1].Text).ToShortDateString();//+ " " + Convert.ToDateTime(r.Cells[1].Text).ToString("HH:mm");
                        //    r.Cells[7].Text = Convert.ToDateTime(r.Cells[7].Text).ToShortDateString();
                        //}
                         r.Cells[1].Text= setDateFormat(r.Cells[1].Text);
                         r.Cells[7].Text = setDateFormat(r.Cells[7].Text);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Binding Gridview fail",ex);
                //if (logger.IsErrorEnabled) logger.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + "() Error.", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
                
            }
        }

        private void Initialize()
        {
            SetLabelText(LabelNoRecord, LabelTitleCheckOwnFile);
            CheckBoxShowDelete.Text = GetResource("cbDisplayDeleted");

            
            if (Session["user"]==null)
            {
                Response.Redirect("LoginMember.aspx");
            }
            else
            {
                
                _DD_MemberAccount = Session["user"].ToString();
            }
            
            LabelNoRecord.Visible = false;
        }

        private List<CheckOwnFilesInfo> getCheckOwnFilesInfo(string MemberId)
        {
            try
            {
                List<CheckOwnFilesInfo> listCheckOwnFile = new List<CheckOwnFilesInfo>();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DDV_GetCheckOwnFileInfo";
                cmd.Parameters.Add(new SqlParameter("@memberId", MemberId));
                cnn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter adt = new SqlDataAdapter(cmd);
                adt.Fill(dt);
                if (dt.Rows.Count == 0) listCheckOwnFile = null;
                else
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        listCheckOwnFile.Add(new CheckOwnFilesInfo
                        {
                            ID = int.Parse(r["ID"].ToString()),
                            Create_date = DateTime.Parse(r["Create_date"].ToString()),
                            Customer_name = r["Customer_name"].ToString(),
                            Customer_Email = r["Customer_Email"].ToString(),
                            Mode_code = int.Parse(r["Mode_code"].ToString()),
                            File_name = r["File_name"].ToString(),
                            File_size = r["File_size"].ToString(),
                            Expiration_date = DateTime.Parse(r["Expiration_date"].ToString()),
                            delete_flag = r["delete_flag"].ToString()
                        });
                    }
                }
                return listCheckOwnFile;
            }
            catch(Exception ex)
            {
                logger.Error("Load DataBase fail");
                if (logger.IsErrorEnabled) logger.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + "() Error.", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
                return null;
            }
            finally
            {
                cnn.Close();
            }
        }

        protected void gridCheckOwnFile_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView grid = (GridView)sender;
                GridViewRow gridrow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell tblcell = new TableCell();

                //add No. header
                tblcell.ColumnSpan = 1;
                tblcell.CssClass = "td_header";
                tblcell.Width = Unit.Pixel(80);
                tblcell.Text = GetResource("ColNo");
                gridrow.Cells.Add(tblcell);

                //add Date header
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text = GetResource("ColDate");
                tblcell.Width = Unit.Pixel(90);
                gridrow.Cells.Add(tblcell);

                //add Customer Name
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text =GetResource("ColCustomerName");
                tblcell.Width = Unit.Pixel(220);
                gridrow.Cells.Add(tblcell);

                //add E-mail Add
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text = GetResource("ColEmailAdd");
                tblcell.Width = Unit.Pixel(220);
                gridrow.Cells.Add(tblcell);

                //add Mode
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text = GetResource("ColMode");
                tblcell.Width = Unit.Pixel(180);
                gridrow.Cells.Add(tblcell);

                //add file name
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text =GetResource("ColFileName");
                tblcell.Width = Unit.Pixel(150);
                gridrow.Cells.Add(tblcell);

                //add file size
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text = GetResource("ColFileSize");
                tblcell.Width = Unit.Pixel(120);
                gridrow.Cells.Add(tblcell);

                //add Period
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text = GetResource("ColPeriod");
                tblcell.Width = Unit.Pixel(90);
                gridrow.Cells.Add(tblcell);

                // add delete
                tblcell = new TableCell();
                tblcell.CssClass = "td_header";
                tblcell.Text = GetResource("ColDelete");
                tblcell.Width = Unit.Pixel(100);
                gridrow.Cells.Add(tblcell);

                //ad header
                grid.Controls[0].Controls.AddAt(0, gridrow);
            }
        }

        protected void gridCheckOwnFile_PreRender(object sender, EventArgs e)
        {
            MergeGridView(gridCheckOwnFile);
        }

        protected void gridCheckOwnFile_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                e.Row.Cells[0].Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
                e.Row.Cells[0].Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                e.Row.Cells[0].ToolTip = "Click to view details";
                e.Row.Cells[0].Attributes["onclick"] = "rowno(" + e.Row.RowIndex + ")";

            }
        }

        private void MergeGridView(GridView gridCheckOwnFile)
       {
           for (int rowIndex = gridCheckOwnFile.Rows.Count - 2; rowIndex >= 0; rowIndex--)
           {
               GridViewRow row = gridCheckOwnFile.Rows[rowIndex];
               GridViewRow previousRow = gridCheckOwnFile.Rows[rowIndex + 1];

               
               for (int i = 4; i <=8; i++)
               {
                   if (row.Cells[i].Text == previousRow.Cells[i].Text && row.Cells[0].Text == previousRow.Cells[0].Text)
                   {
                       row.Cells[i].RowSpan = previousRow.Cells[i].RowSpan < 2 ? 2 : previousRow.Cells[i].RowSpan + 1;
                       previousRow.Cells[i].Visible = false;
                   }
               }
               if (row.Cells[1].Text == previousRow.Cells[1].Text && row.Cells[0].Text == previousRow.Cells[0].Text)
               {
                   row.Cells[1].RowSpan = previousRow.Cells[1].RowSpan < 2 ? 2 : previousRow.Cells[1].RowSpan + 1;
                   previousRow.Cells[1].Visible = false;

               }
             
               if (row.Cells[3].Text == previousRow.Cells[3].Text && row.Cells[2].Text == previousRow.Cells[2].Text && row.Cells[0].Text == previousRow.Cells[0].Text )
               {
                   row.Cells[3].RowSpan = previousRow.Cells[3].RowSpan < 2 ? 2 : previousRow.Cells[3].RowSpan + 1;
                   previousRow.Cells[3].Visible = false;

               }
               if (row.Cells[2].Text == previousRow.Cells[2].Text && row.Cells[0].Text == previousRow.Cells[0].Text)
               {
                   row.Cells[2].RowSpan = previousRow.Cells[2].RowSpan < 2 ? 2 : previousRow.Cells[2].RowSpan + 1;
                   previousRow.Cells[2].Visible = false;
               }
               if (row.Cells[0].Text == previousRow.Cells[0].Text)
               {
                   row.Cells[0].RowSpan = previousRow.Cells[0].RowSpan < 2 ? 2 : previousRow.Cells[0].RowSpan + 1;
                   previousRow.Cells[0].Visible = false;
               }
               
           }
       }

        protected void CheckBoxShowDelete_CheckedChanged(object sender, EventArgs e)
        {
            BindingGridView();
        }

        protected void gridCheckOwnFile_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridCheckOwnFile.PageIndex = e.NewPageIndex;
            BindingGridView();
        }
  
    }
}