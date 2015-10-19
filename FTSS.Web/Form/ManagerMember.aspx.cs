using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FTSS.Domain;
using FTSS.Utilities;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Resources;
using System.Text;

namespace FTSS.Web.Form
{
    public partial class ManagerMember : FTSSPageUtil
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("LoginMember.aspx");
            }
            if (!IsPostBack)
            {
                InitLanguage();
                FillDropdownlistRole();
                FillDropdownlistAdmin();
                ButtonSearch_Click(sender, e);
                SetButtonText(ButtonSave, ButtonSearch, ButtonClear, ButtonCancel, ButtonDelete, ButtonEdit, ButtonRegister);
                SetLabelText(LabelAccount, lblPassword, LabelEmail, LabelRole, LabelManagement, LabelEdit, lblAccount, lblName, lblRole, lblEmail);
                PanelMember.Visible = false;
            }
        }
        private void FillDropdownlistRole()
        {
            DropDownListRole.Items.Add(new ListItem(GetResource("All"), "0"));
            DropDownListRole.Items.Add(new ListItem(GetResource("Waiting"), ""));
            DropDownListRole.Items.Add(new ListItem(GetResource("User"), "2"));
            DropDownListRole.Items.Add(new ListItem(GetResource("Admin"), "1"));

        }
        private void FillDropdownlistAdmin()
        {
            DropDownListAdmin.Items.Add(new ListItem(GetResource("Waiting"), ""));
            DropDownListAdmin.Items.Add(new ListItem(GetResource("User"), "2"));
            DropDownListAdmin.Items.Add(new ListItem(GetResource("Admin"), "1"));
        }
        public DataTable GetMember(string account)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                Conn.Open();
                using (SqlCommand cmd = new SqlCommand("DDV_GetByIdMember", Conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@account", account));
                    DataTable dt = new DataTable();
                    SqlDataAdapter adt = new SqlDataAdapter(cmd);
                    adt.Fill(dt);
                    Conn.Close();
                    return dt;
                }
            }
        }

        private void FillDataToGridview()
        {
            List<Member> listobj = new List<Member>();
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                Conn.Open();
                using (SqlCommand cmd = new SqlCommand("DDV_SearchMember", Conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    Member obj = new Member();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            listobj.Add(obj.MemberIDataReader(dr));
                        }
                    }
                    dr.Close();
                    obj = null;
                    #region Check list member Null
                    if (listobj.Count < 1)
                    {

                        Member member = new Member();
                        member.Account = "";
                        member.Name = "";
                        member.Email = "";
                        member.Pw = "";
                        member.Permission = null;

                        listobj.Add(member);
                        GridViewMember.DataSource = listobj;
                        GridViewMember.DataBind();
                        if (listobj.Count == 1 && member.Account == "")
                        {
                            //LabelNoRecord.Visible = true;
                            GridViewMember.Rows[0].Visible = false;
                            return;
                        }
                    }
                    #endregion
                    else
                    {
                        listobj = listobj.Where(c => (TextBoxAccount.Text == string.Empty || c.Account.ToLower().Contains(TextBoxAccount.Text.ToLower()))
                        && (TextBoxEmail.Text == string.Empty || c.Email.ToLower().Contains(TextBoxEmail.Text.ToLower())) 
                        && (DropDownListRole.SelectedValue == "0" || c.Permission.ToString() == (DropDownListRole.SelectedValue))).ToList();
                        #region Check list member Null
                        if (listobj.Count < 1)
                        {

                            Member member = new Member();
                            member.Account = "";
                            member.Name = "";
                            member.Email = "";
                            member.Pw = "";
                            member.Permission = null;

                            listobj.Add(member);
                            GridViewMember.DataSource = listobj;
                            GridViewMember.DataBind();
                            if (listobj.Count == 1 && member.Account == "")
                            {
                                //LabelNoRecord.Visible = true;
                                GridViewMember.Rows[0].Visible = false;
                                return;
                            }
                        }
                        #endregion
                        GridViewMember.DataSource = listobj;
                        GridViewMember.DataBind();
                        foreach (GridViewRow row in GridViewMember.Rows)
                        {
                            if (row.Cells[4].Text == "1")
                                row.Cells[4].Text = GetResource("Admin");
                            else if (row.Cells[4].Text == "2")
                                row.Cells[4].Text = GetResource("User");
                            else if (Common.GetRowString(row.Cells[4].Text) == "")
                                row.Cells[4].Text = GetResource("Waiting");
                        }

                    }
                }
            }
        }
        protected void GridViewMember_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewMember.PageIndex = e.NewPageIndex;
            FillDataToGridview();
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(TextBoxAccount.Text))
                    TextBoxAccount.Text = string.Empty;
                if (String.IsNullOrEmpty(TextBoxEmail.Text))
                    TextBoxEmail.Text = string.Empty;
                if (DropDownListRole.SelectedIndex == 0)
                    DropDownListRole.SelectedValue = "0";
                FillDataToGridview();
            }
            catch (Exception ex)
            {
                logger.Error("bind gridviewMember fail", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), "" + ex.Message + "") + "\");");
            }
        }

        private void ResetInput()
        {
            txtAccount.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            DropDownListAdmin.SelectedIndex = 0;
        }

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            TextBoxAccount.Text = "";
            TextBoxEmail.Text = "";
            DropDownListRole.SelectedIndex = 0;
        }

        protected void ButtonRegister_Click(object sender, EventArgs e)
        {
            PanelMember.Visible = true;
            PanelGridManager.Visible = false;
            txtAccount.Enabled = true;
            txtPassword.Enabled = true;
            ButtonSave.Text = GetResource("ButtonAdd");
            ButtonDelete.Enabled = false;
            lblEmail.Text = GetResource("lblEmail");
            LabelEdit.Text = GetResource("LabelRegister");
            lblPassword.Visible = true;
            txtPassword.Visible = true;
            ResetInput();
        }

        protected void GridViewMember_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                InitLanguage();
                GridView gridview = (GridView)sender;
                GridViewRow gridviewRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell tableCell = new TableCell();

                //add Status header
                tableCell.ColumnSpan = 1;
                tableCell = new TableCell();
                tableCell.CssClass = "td_header";
                tableCell.Width = Unit.Pixel(10);
                gridviewRow.Cells.Add(tableCell);

                //add Account header
                tableCell = new TableCell();
                tableCell.CssClass = "td_header";
                tableCell.Width = Unit.Pixel(100);
                tableCell.Text = GetResource("ColAccount");
                gridviewRow.Cells.Add(tableCell);

                //add Name header
                tableCell = new TableCell();
                tableCell.CssClass = "td_header";
                tableCell.Width = Unit.Pixel(100);
                tableCell.Text = GetResource("ColName");
                gridviewRow.Cells.Add(tableCell);

                //add Email header
                tableCell = new TableCell();
                tableCell.CssClass = "td_header";
                tableCell.Width = Unit.Pixel(150);
                tableCell.Text = GetResource("ColEmail");
                gridviewRow.Cells.Add(tableCell);


                //add Password header
                //tableCell = new TableCell();
                //tableCell.CssClass = "td_header";
                //tableCell.Width = Unit.Pixel(150);
                //tableCell.Text = GetResource("ColPassword");
                //gridviewRow.Cells.Add(tableCell);

                //add Role header
                tableCell = new TableCell();
                tableCell.CssClass = "td_header";
                tableCell.Width = Unit.Pixel(50);
                tableCell.Text = GetResource("ColPermission");
                gridviewRow.Cells.Add(tableCell);

                //ad header
                gridview.Controls[0].Controls.AddAt(0, gridviewRow);
            }
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            if (ButtonSave.Text == GetResource("ButtonUpdate"))
                UpdateMember();
            if (ButtonSave.Text == GetResource("ButtonAdd"))
                InsertMember();
            FillDataToGridview();
        }

        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    string account = txtAccount.Text;
                    using (SqlCommand cmd = new SqlCommand("DDV_DeleteMember", Conn))
                    {
                        Conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@account", account);
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                        Response.Redirect("ManagerMember.aspx");
                    }
                }
                catch (Exception ex)
                {
                    LabelMessage.Text = ex.Message;
                    logger.Error("buttonDelete Account fail ", ex);
                }
            }
            PanelMember.Visible = false;
            PanelGridManager.Visible = true;
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
        #region Update Member
        private void UpdateMember()
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    string memberMail = txtEmail.Text;
                    List<UserMail> listMember = new List<UserMail>();
                    UserMail accountMailActivated = null;
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_UpdateMember", Conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@account", txtAccount.Text);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@permission", DropDownListAdmin.SelectedItem.Value.ToString());
                        var result = (int)cmd.ExecuteNonQuery();
                        Conn.Close();
                        if (result > 0)
                        {
                            accountMailActivated = new UserMail(txtAccount.Text, txtName.Text, memberMail);
                            PanelGridManager.Visible = true;
                            PanelMember.Visible = false;

                            if (DropDownListAdmin.SelectedValue != null)
                            {
                                
                                
                                string subject = Common.GetResourceString("ActivatedAccount");
                                string mailBody = GetMailBody("Notify_account_activated.txt");
                                listMember.Add(accountMailActivated);
                                foreach (UserMail mail in listMember)
                                {
                                    mail.SendEmail(accountMailActivated, mail,subject, mailBody);
                                }
                            }
                        }
                        else
                        {
                            RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), GetResource("UpdateFail")) + "\");");
                        }

                    }

                }
                catch (SqlException ex)
                {
                    LabelMessage.Text = ex.Message;
                    logger.Error("Button update account error ", ex);
                }
            }
        }
        #endregion
        public void SendmailActived()
        {
            List<UserMail> listMember = new List<UserMail>();
            UserMail userMail = null;
            string subject = Common.GetResourceString("ActivatedAccount");
            string mailBody = GetMailBody("Notify_account_activated");
            foreach (UserMail mail in listMember)
            {
                mail.SendEmail(userMail, subject, mailBody);
            }
        }
        #region Insert member
        private void InsertMember()
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    //Encrypt password
                    String encrypt = GenerateHashWithSalt(txtPassword.Text, txtAccount.Text);
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_InsertMember", Conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@account", txtAccount.Text);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@password", encrypt);
                        cmd.Parameters.AddWithValue("@permission", DropDownListAdmin.SelectedItem.Value.ToString());
                        var result = (int)cmd.ExecuteNonQuery();
                        Conn.Close();
                        if (result > 0)
                        {
                            PanelGridManager.Visible = true;
                            PanelMember.Visible = false;
                        }
                        else
                        {
                            RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), GetResource("RegisterFail")) + "\");");
                        }
                    }

                }
                catch (SqlException ex)
                {
                    LabelMessage.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    logger.Error("Register page error ", ex);
                }
            }
        }
        #endregion
        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            PanelGridManager.Visible = true;
            PanelMember.Visible = false;
        }

        protected void ButtonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var chkAccount = (from GridViewRow row in GridViewMember.Rows
                                  where ((CheckBox)row.FindControl("chkStatus")).Checked
                                  select GridViewMember.DataKeys[row.RowIndex].Value.ToString()).ToList();

                if (chkAccount.Count == 0)
                {
                    RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), GetResource("MSG_NONE_SELECTED_ITEM")) + "\");");
                }

                else if (chkAccount.Count > 1)
                {
                    RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), GetResource("MSG_MORE_ONE_SELECTED_ITEM")) + "\");");
                }
                else
                {
                    DataTable dt = GetMember(chkAccount[0].ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        txtAccount.Text = chkAccount[0].ToString();
                        txtName.Text = row[1].ToString();
                        txtEmail.Text = row[2].ToString();
                        //txtPassword.Text = row[3].ToString();
                        if (row[4] == DBNull.Value)
                            DropDownListAdmin.SelectedValue = "";
                        else
                            DropDownListAdmin.SelectedValue = row[4].ToString();
                    }
                    txtAccount.Enabled = false;
                    txtPassword.Visible = false;
                    lblPassword.Visible = false;
                    PanelGridManager.Visible = false;
                    PanelMember.Visible = true;
                    ButtonSave.Text = GetResource("ButtonUpdate");
                    LabelEdit.Text = GetResource("LabelEdit");
                    ButtonDelete.Enabled = true;
                    RequiredFieldValidator5.Visible = false;
                    if (Session["admin"].ToString() == txtAccount.Text)
                    {
                        DropDownListAdmin.Enabled = false;
                    }
                    else
                    {
                        DropDownListAdmin.Enabled = true;
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                logger.Error("Error Button Edit ", ex);
                RegisterStartupScript("alert(\"" + GetJSMessage(GetResource("TITLE_ERROR"), ex.Message) + "\");");
            }
        }
    }
}