using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FTSS.Utilities;

namespace FTSS.Web.Form
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            lBtnLoginMem.Text = Common.GetResourceString("lBtnLoginMem");
            lBtnLoginCus.Text = Common.GetResourceString("lBtnLoginCus");
            lBtnlogout.Text = Common.GetResourceString("lBtnlogout");
            lblTitle.Text = Common.GetResourceString("lblTitle");

            if (Session["user"] != null || Session["customer"] != null)
            {
                MultiView.ActiveViewIndex = 1;
            }
            else if (Request.QueryString["customerId"] != null || Request.QueryString["ID"]!=null)
            {
                MultiView.ActiveViewIndex = 1;
            }
            else
            {
                MultiView.ActiveViewIndex = 0;
            }

            if (!IsPostBack)
            {
                HttpCookie cookie = Request.Cookies["CurrentLanguage"];
                if (cookie != null && cookie.Value != null)
                {
                    if (cookie.Value.IndexOf("en-") >= 0)
                    {
                        ImgBtn_En.Enabled = false;
                        ImgBtn_Jp.Enabled = true;
                    }
                    else
                    {
                        ImgBtn_En.Enabled = true;
                        ImgBtn_Jp.Enabled = false;
                    }
                }
            }
        }
        protected void ImgBtn_En_Click(object sender, ImageClickEventArgs e)
        {
            HttpCookie cookie = new HttpCookie("CurrentLanguage");
            cookie.Value = "en-US";
            cookie.Expires = DateTime.Now.AddMonths(6);
            Response.SetCookie(cookie);
            Response.Redirect(Request.RawUrl);
        }
        protected void ImgBtn_Jp_Click(object sender, ImageClickEventArgs e)
        {
            HttpCookie cookie = new HttpCookie("CurrentLanguage");
            cookie.Value = "ja-JP";
            Response.SetCookie(cookie);
            cookie.Expires = DateTime.Now.AddMonths(6);
            Response.Redirect(Request.RawUrl);
        }

        protected void lBtnLoginMem_Click(object sender, EventArgs e)
        {
            Server.Transfer("LoginMember.aspx");
        }

        protected void lBtnLoginCus_Click(object sender, EventArgs e)
        {
            Server.Transfer("LoginCustomer.aspx");
        }

        protected void lBtnlogout_Click(object sender, EventArgs e)
        {
            Session.Remove("user");
            Session.Remove("customer");


            // To clear Query string value
            PropertyInfo isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

            // make collection editable
            isreadonly.SetValue(this.Request.QueryString, false, null);

            // remove
            if(this.Request.QueryString["ID"] != null)
              this.Request.QueryString.Remove("ID");

    
            Server.Transfer("LoginMember.aspx");           

        }
        public String MyString
        {
            get { return (String)Session["admin"]; }
            set { Session["admin"] = value; }
        }
    }
}