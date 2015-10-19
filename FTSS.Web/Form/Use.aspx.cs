using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FTSS.Web.Form
{
    public partial class Use : FTSSPageUtil
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack) 
              SetButtonText(ButtonClose);
        }

        protected void ButtonClose_Click(object sender, EventArgs e)
        {
            Server.Transfer("LoginMember.aspx"); 
        }
    }
}